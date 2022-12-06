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
using System.Windows.Input;

namespace SmartMirror.ViewModels;

public class MainTabbedPageViewModel : BaseViewModel
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IDevicesService _devicesService;
    private readonly IRoomsService _roomsService;
    private readonly IScenariosService _scenariosService;
    private readonly IAutomationService _automationService;
    private readonly IAqaraMessanger _aqaraMessanger;

    private OpenFullScreenCameraEvent _openFullScreenVideoEvent;

    private int _buttonCount;
    private bool _isFirstTime = true;

    public MainTabbedPageViewModel(
        INavigationService navigationService,
        IEventAggregator eventAggregator,
        IDevicesService devicesService,
        IRoomsService roomsService,
        IAutomationService automationService,
        IScenariosService scenariosService,
        IAqaraMessanger aqaraMessanger)
        : base(navigationService)
    {
        _eventAggregator = eventAggregator;
        _devicesService = devicesService;
        _roomsService = roomsService;
        _scenariosService = scenariosService;
        _automationService = automationService;
        _aqaraMessanger = aqaraMessanger;
        _aqaraMessanger.StartListening();

        _openFullScreenVideoEvent = _eventAggregator.GetEvent<OpenFullScreenCameraEvent>();
        _openFullScreenVideoEvent.Subscribe(OpenFullscreenVideoEventHandler);
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
    }

    private Task OnSettingsCommandAsync()
    {
        return NavigationService.CreateBuilder()
            .AddSegment<SettingsPageViewModel>()
            .NavigateAsync();
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

