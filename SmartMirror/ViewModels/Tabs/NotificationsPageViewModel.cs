using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Notifications;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class NotificationsPageViewModel : BaseTabViewModel
{
    private readonly INotificationsService _notificationsService;

    public NotificationsPageViewModel(
        INotificationsService notificationsService,
        INavigationService navigationService)
        : base(navigationService)
    {
        _notificationsService = notificationsService;

        Title = "Notifications";
        DataState = EPageState.Loading;

        ConnectivityChanged += OnConnectivityChanged;
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

    #endregion

    #region -- Overrides --

    public async override Task InitializeAsync(INavigationParameters parameters)
    {
        await base.InitializeAsync(parameters);

        await LoadNotificationsAsync();
    }

    #endregion

    #region -- Private helpers --

    private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            await LoadNotificationsAsync();
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private async Task OnRefreshNotificationsCommandAsync()
    {
        await LoadNotificationsAsync();

        IsNotificationsRefreshing = false;
    }

    private async Task LoadNotificationsAsync()
    {
        var resultOfGettingNotifications = await _notificationsService.GetNotificationsGroupedByDayAsync();

        if (resultOfGettingNotifications.IsSuccess)
        {
            Notifications = new(resultOfGettingNotifications.Result);

            DataState = EPageState.Complete;
        }
    }

    #endregion
}

