using CommunityToolkit.Maui.Alerts;
using SmartMirror.Helpers;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Automation;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Rooms;
using SmartMirror.Services.Scenarios;
using SmartMirror.Services.Notifications;
using SmartMirror.Views;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using System.Windows.Input;
using SmartMirror.Views.Dialogs;

namespace SmartMirror.ViewModels;

public class MainTabbedPageViewModel : BaseViewModel
{
    private readonly IDevicesService _devicesService;
    private readonly IDialogService _dialogService;
    private readonly IRoomsService _roomsService;
    private readonly IScenariosService _scenariosService;
    private readonly INotificationsService _notificationsService;
    private readonly IAutomationService _automationService;
    private readonly IAqaraMessanger _aqaraMessanger;
    
    private int _buttonCount;
    private bool _isFirstTime = true;

    public MainTabbedPageViewModel(
        INavigationService navigationService,
        IDevicesService devicesService,
        IDialogService dialogService,
        IRoomsService roomsService,
        INotificationsService notificationsService,
        IAutomationService automationService,
        IScenariosService scenariosService,
        IAqaraMessanger aqaraMessanger)
        : base(navigationService)
    {
        _devicesService = devicesService;
        _dialogService = dialogService;
        _roomsService = roomsService;
        _scenariosService = scenariosService;
        _notificationsService = notificationsService;
        _automationService = automationService;
        _aqaraMessanger = aqaraMessanger;

        _aqaraMessanger.StartListening();
        
        _notificationsService.NotificationReceived += OnShowEmergencyNotificationDialogAsync;
    }

    #region -- Public properties --

    private ICommand _settingsCommand;
    public ICommand SettingsCommand => _settingsCommand ??= SingleExecutionCommand.FromFunc(OnSettingsCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

    #endregion

    #region -- Overrides --

    public override void Destroy()
    {
        _aqaraMessanger.StopListening();

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

    private async void OnShowEmergencyNotificationDialogAsync(object sender, NotificationGroupItemBindableModel e)
    {
        var result = await _dialogService.ShowDialogAsync(nameof(EmergencyNotificationDialog), new DialogParameters
        {
            { Constants.DialogsParameterKeys.SCENARIO, "j" },
        });
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

    #endregion
}

