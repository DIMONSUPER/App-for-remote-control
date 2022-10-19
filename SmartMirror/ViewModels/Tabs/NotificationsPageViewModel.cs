using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Notifications;
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

    public override void OnAppearing()
    {
        base.OnAppearing();

        if (!IsDataLoading)
        {
            DataState = EPageState.Loading;

            Task.Run(() => LoadNotificationsAndChangeStateAsync());
        }
    }

    protected override void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            if (!IsDataLoading && DataState != EPageState.Complete)
            {
                DataState = EPageState.Loading;

                Task.Run(() => LoadNotificationsAndChangeStateAsync());
            }
        }
        else
        {
            IsNotificationsRefreshing = false;

            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var timeToStopUpdating = DateTime.Now.AddSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.Repeate(LoadNotificationsAsync, canExecute: () => DateTime.Now < timeToStopUpdating);

            if (IsInternetConnected)
            {
                DataState = isDataLoaded
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
    }

    private async Task OnRefreshNotificationsCommandAsync()
    {
        if (!IsDataLoading)
        {
            await LoadNotificationsAndChangeStateAsync();

            IsNotificationsRefreshing = false; 
        }
    }

    private async Task LoadNotificationsAndChangeStateAsync()
    {
        if (IsInternetConnected)
        {
            var isDataLoaded = await LoadNotificationsAsync();

            if (IsInternetConnected)
            {
                DataState = isDataLoaded
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private async Task<bool> LoadNotificationsAsync()
    {
        bool isLoaded = false;
        
        IsDataLoading = true;

        if (IsInternetConnected)
        {
            await Task.Delay(1000);

            var resultOfGettingNotifications = await _notificationsService.GetNotificationsAsync();

            if (resultOfGettingNotifications.IsSuccess)
            {
                var allNotifications = resultOfGettingNotifications.Result.OrderByDescending(row => row.LastActivityTime);

                var notificationGroups = GetNotificationGroups(allNotifications);

                Notifications = new(notificationGroups);

                isLoaded = true;
            }
        }

        IsDataLoading = false;

        return isLoaded;
    }

    private ObservableCollection<INotificationGroupItemModel> GetNotificationGroups(IOrderedEnumerable<NotificationModel> notifications)
    {
        var lastTitleGroup = string.Empty;

        var notificationGrouped = new ObservableCollection<INotificationGroupItemModel>();

        foreach (var notificafication in notifications)
        {
            var titleGroup = notificafication.LastActivityTime.ToString(Constants.Formats.DATE_FORMAT);

            if (lastTitleGroup != titleGroup)
            {
                notificationGrouped.Add(new NotificationGroupTitleBindableModel()
                {
                    Title = titleGroup,
                });

                lastTitleGroup = titleGroup;
            }

            notificationGrouped.Add(_mapperService.Map<NotificationGroupItemBindableModel>(notificafication));
        }

        return notificationGrouped;
    }

    #endregion
}
