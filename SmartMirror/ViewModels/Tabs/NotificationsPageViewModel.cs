using SmartMirror.Models;
using SmartMirror.Services.Notifications;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class NotificationsPageViewModel : BaseTabViewModel
{
    private readonly INotificationsService _notificationsService;

    public NotificationsPageViewModel(INotificationsService notificationsService)
    {
        Title = "Notifications";
        _notificationsService = notificationsService;
    }

    #region -- Public properties --

    private ObservableCollection<NotificationsGroupedByDayModel> _notifications;
    public ObservableCollection<NotificationsGroupedByDayModel> Notifications
    {
        get => _notifications;
        set => SetProperty(ref _notifications, value);
    }

    private bool _isRefreshingNotifications;
    public bool IsRefreshingNotifications
    {
        get => _isRefreshingNotifications;
        set => SetProperty(ref _isRefreshingNotifications, value);
    }

    public ICommand RefreshNotifications => new Command(OnRefreshNotificationsCommandAsync);

    #endregion

    #region -- Overrides --

    public async override Task InitializeAsync(INavigationParameters parameters)
    {
        await base.InitializeAsync(parameters);

        await LoadNotificationsAsync();
    }

    #endregion

    #region -- Private helpers --

    private async void OnRefreshNotificationsCommandAsync()
    {
        await LoadNotificationsAsync();

        IsRefreshingNotifications = false;
    }

    private async Task LoadNotificationsAsync()
    {
        var resultOfGettingNotifications = await _notificationsService.GetNotificationsGroupedByDayAsync();

        if (resultOfGettingNotifications.IsSuccess)
        {
            Notifications = new(resultOfGettingNotifications.Result);
        }
    }

    #endregion
}

