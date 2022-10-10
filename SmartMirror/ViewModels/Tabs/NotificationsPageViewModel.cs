using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Notifications;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class NotificationsPageViewModel : BaseTabViewModel
{
    private readonly INotificationsService _notificationsService;
    private readonly IDialogService _dialogService;

    public NotificationsPageViewModel(
        INotificationsService notificationsService,
        IDialogService dialogService,
        INavigationService navigationService)
        : base(navigationService)
    {
        _notificationsService = notificationsService;
        _dialogService = dialogService;

        Title = "Notifications";
        DataState = EPageState.Loading;
    }

    #region -- Public properties --

    private ObservableCollection<NotificationsGroupedByDayModel> _notifications;
    public ObservableCollection<NotificationsGroupedByDayModel> Notifications
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
    public ICommand RefreshNotificationsCommand => _refreshNotificationsCommand ??= SingleExecutionCommand.FromFunc(OnRefreshNotificationsCommandAsync);

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
                { Constants.DialogsParameterKeys.TITLE, LocalizationResourceManager.Instance["SomethingWentWrong"] },
                { Constants.DialogsParameterKeys.DESCRIPTION, LocalizationResourceManager.Instance["NoInternetConnection"] },
            };

            await _dialogService.ShowDialogAsync(nameof(ErrorDialog), dialogParameters);
        });

        IsNotificationsRefreshing = false;
    }

    private async Task LoadNotificationsAsync(Action onFailure)
    {
        if (IsInternetConnected)
        {
            var resultOfGettingNotifications = await _notificationsService.GetNotificationsGroupedByDayAsync();

            if (resultOfGettingNotifications.IsSuccess)
            {
                Notifications = new(resultOfGettingNotifications.Result);

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

