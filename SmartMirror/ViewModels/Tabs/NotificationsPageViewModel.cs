using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Notifications;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class NotificationsPageViewModel : BaseTabViewModel
{
    private readonly INotificationsService _notificationsService;
    private readonly IDialogService _dialogService;
    private readonly IMapperService _mapperService;

    public NotificationsPageViewModel(
        INotificationsService notificationsService,
        IDialogService dialogService,
        IMapperService mapperService,
        INavigationService navigationService)
        : base(navigationService)
    {
        _notificationsService = notificationsService;
        _dialogService = dialogService;
        _mapperService = mapperService;

        Title = "Notifications";
        DataState = EPageState.Loading;
    }

    #region -- Public properties --

    private ObservableCollection<INotificationGroupItemModel> _notifications;
    public ObservableCollection<INotificationGroupItemModel> Notifications
    {
        get => _notifications;
        set => SetProperty(ref _notifications, value);
    }

    private bool _isNotificationsRefreshing;
    public bool IsNotificationsRefreshing
    {
        get => _isNotificationsRefreshing;
        set => SetProperty(ref _isNotificationsRefreshing, value);
    }

    private ICommand _refreshNotificationsCommand;
    public ICommand RefreshNotificationsCommand => _refreshNotificationsCommand ??= SingleExecutionCommand.FromFunc(OnRefreshNotificationsCommandAsync, delayMillisec: 0);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    #endregion

    #region -- Overrides --

    public async override Task InitializeAsync(INavigationParameters parameters)
    {
        await base.InitializeAsync(parameters);

        await LoadNotificationsAsync(() => DataState = EPageState.NoInternet);
    }

    #endregion

    #region -- Private helpers --

    private async Task OnTryAgainCommandAsync()
    {
        DataState = EPageState.NoInternetLoader;

        await Task.Delay(1000);

        await LoadNotificationsAsync(() => DataState = EPageState.NoInternet);
    }

    private async Task OnRefreshNotificationsCommandAsync()
    {
        await LoadNotificationsAsync(async () =>
        {
            var dialogParameters = new DialogParameters()
            {
                { Constants.DialogsParameterKeys.TITLE, Strings.SomethingWentWrong },
                { Constants.DialogsParameterKeys.DESCRIPTION, Strings.NoInternetConnection },
            };

            await _dialogService.ShowDialogAsync(nameof(ErrorDialog), dialogParameters);
        });

        IsNotificationsRefreshing = false;
    }

    private async Task LoadNotificationsAsync(Action onFailure)
    {
        if (IsInternetConnected)
        {
            var resultOfGettingNotifications = await _notificationsService.GetNotificationsAsync();

            if (resultOfGettingNotifications.IsSuccess)
            {
                var allNotifications = resultOfGettingNotifications.Result.OrderByDescending(row => row.LastActivityTime);

                var lastTitleGroup = string.Empty;
                var notificationGroped = new ObservableCollection<INotificationGroupItemModel>();

                foreach (var notificafication in allNotifications)
                {
                    var titleGroup = notificafication.LastActivityTime.ToString(Constants.Formats.DATE_FORMAT);

                    if (lastTitleGroup != titleGroup)
                    {
                        notificationGroped.Add(new NotificationGroupTitle()
                        {
                            Name = titleGroup,
                        });

                        lastTitleGroup = titleGroup;
                    }

                    notificationGroped.Add(await _mapperService.MapAsync<NotificationGroupItem>(notificafication));
                }

                Notifications = new(notificationGroped);

                DataState = EPageState.Complete;
            }
            else if (!IsInternetConnected)
            {
                onFailure();
            }
        }
        else
        {
            onFailure();
        }
    }

    #endregion
}

