using CommunityToolkit.Maui.Alerts;
using SmartMirror.Helpers;
using SmartMirror.Helpers.Events;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Automation;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Rooms;
using SmartMirror.Services.Scenarios;
using SmartMirror.Services.Notifications;
using SmartMirror.Views;
using System.Windows.Input;
using SmartMirror.Views.Dialogs;
using System.ComponentModel;

namespace SmartMirror.ViewModels;

public class MainTabbedPageViewModel : BaseViewModel
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IDevicesService _devicesService;
    private readonly IDialogService _dialogService;
    private readonly IRoomsService _roomsService;
    private readonly IScenariosService _scenariosService;
    private readonly INotificationsService _notificationsService;
    private readonly IAutomationService _automationService;
    private readonly IAqaraMessanger _aqaraMessanger;

    private OpenFullScreenCameraEvent _openFullScreenVideoEvent;    

    private int _buttonCount;
    private bool _isFirstTime = true;

    public MainTabbedPageViewModel(
        INavigationService navigationService,
        IEventAggregator eventAggregator,
        IDevicesService devicesService,
        IDialogService dialogService,
        IRoomsService roomsService,
        INotificationsService notificationsService,
        IAutomationService automationService,
        IScenariosService scenariosService,
        IAqaraMessanger aqaraMessanger)
        : base(navigationService)
    {
        _eventAggregator = eventAggregator;
        _devicesService = devicesService;
        _dialogService = dialogService;
        _roomsService = roomsService;
        _scenariosService = scenariosService;
        _notificationsService = notificationsService;
        _automationService = automationService;
        _aqaraMessanger = aqaraMessanger;

        _aqaraMessanger.StartListening();

        _openFullScreenVideoEvent = _eventAggregator.GetEvent<OpenFullScreenCameraEvent>();
        _openFullScreenVideoEvent.Subscribe(OpenFullscreenVideoEventHandler);
        
        _notificationsService.AllNotificationsChanged += OnShowEmergencyNotificationDialogAsync;
    }

    #region -- Public properties --

    private ICommand _settingsCommand;
    public ICommand SettingsCommand => _settingsCommand ??= SingleExecutionCommand.FromFunc(OnSettingsCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

    #endregion

    #region -- Overrides --

    public override void Destroy()
    {
        _aqaraMessanger.StopListening();
        _openFullScreenVideoEvent.Unsubscribe(OpenFullscreenVideoEventHandler);

        base.Destroy();
    }

    public override async void OnNavigatedTo(INavigationParameters parameters)
    {
        base.OnNavigatedTo(parameters);

        if (_isFirstTime)
        {
            _isFirstTime = false;

            await UpdateAllAqaraDataAsync();

            await DisplayDoorbellDialogAsync();
        }
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        base.OnConnectivityChanged(sender, e);

        if (IsInternetConnected)
        {
            await UpdateAllAqaraDataAsync();
        }
    }

    public override bool OnBackButtonPressed()
    {
        if (_buttonCount < 1)
        {
            var interval = TimeSpan.FromMilliseconds(500);
            Application.Current.Dispatcher.StartTimer(interval, GetCountBackButtonPresses);
        }

        _buttonCount++;

        return true;
    }

    #endregion

    #region -- Private helpers --

    private async Task UpdateAllAqaraDataAsync()
    {
        await _devicesService.DownloadAllDevicesWithSubInfoAsync();
        await _roomsService.DownloadAllRoomsAsync();
        await _scenariosService.DownloadAllScenariosAsync();
        await _automationService.DownloadAllAutomationsAsync();
        await _notificationsService.DownloadAllNotificationsAsync();
    }

    private Task OnSettingsCommandAsync()
    {
        return NavigationService.CreateBuilder()
            .AddSegment<SettingsPageViewModel>()
            .NavigateAsync();
    }

    private async void OnShowEmergencyNotificationDialogAsync(object sender, NotificationGroupItemBindableModel notification)
    {
        if (notification is not null && notification.IsEmergencyNotification)
        {
            _notificationsService.AllNotificationsChanged -= OnShowEmergencyNotificationDialogAsync;

            var result = await _dialogService.ShowDialogAsync(nameof(EmergencyNotificationDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.NOTIFICATION, notification },
            });

            _notificationsService.AllNotificationsChanged += OnShowEmergencyNotificationDialogAsync;
        }
    }

    private async Task DisplayDoorbellDialogAsync()
    {
        //TODO Delete when doorbell is implemented

        var allDevices = await _devicesService.GetAllSupportedDevicesAsync();

        var mockDoorbell = allDevices.FirstOrDefault(row => row.DeviceId == "5000");

        // mockDoorbell is null if the token has expired or allDevices.Count == 0

        if (mockDoorbell is not null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await _dialogService.ShowDialogAsync(nameof(DoorBellDialog), new DialogParameters()
            {
                { Constants.DialogsParameterKeys.ACCESSORY, mockDoorbell },
            }));
        }
    }

    private bool GetCountBackButtonPresses()
    {
        if (_buttonCount > 1)
        {
            Application.Current.Quit();
        }
        else
        {
            Toast.Make(Strings.NeedsTwoTaps).Show();
        }

        _buttonCount = 0;

        return false;
    }

    private void OpenFullscreenVideoEventHandler(CameraBindableModel camera)
    {
        NavigationService.CreateBuilder()
            .AddSegment<FullScreenCameraPageViewModel>()
            .AddParameter(Constants.DialogsParameterKeys.CAMERA, camera)
            .Navigate();
    }

    #endregion
}

