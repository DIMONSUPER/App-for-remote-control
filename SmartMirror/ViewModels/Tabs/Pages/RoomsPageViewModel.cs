using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using SmartMirror.Services.Rooms;
using SmartMirror.ViewModels.Tabs.Details;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class RoomsPageViewModel : BaseTabViewModel
{
    private readonly ISmartHomeMockService _smartHomeMockService;
    private readonly IMapperService _mapperService;
    private readonly IAqaraService _aqaraService;
    private readonly IDialogService _dialogService;
    private readonly IRoomsService _roomsService;
    private readonly IDevicesService _devicesService;

    private bool _isPageFocused;
    private bool _needReloadDevice;

    public RoomsPageViewModel(
        ISmartHomeMockService smartHomeMockService,
        INavigationService navigationService,
        IMapperService mapperService,
        IAqaraService aqaraService,
        IRoomsService roomsService,
        IDialogService dialogService,
        IDevicesService devicesService)
        : base(navigationService)
    {
        _smartHomeMockService = smartHomeMockService;
        _mapperService = mapperService;
        _aqaraService = aqaraService;
        _dialogService = dialogService;
        _roomsService = roomsService;
        _devicesService = devicesService;

        Title = "Rooms";

        DataState = EPageState.LoadingSkeleton;

        Task.Run(LoadRoomsAndDevicesAndChangeStateAsync);

        _roomsService.AllRoomsChanged += OnAllRoomsChanged;
        _devicesService.AllDevicesChanged += OnAllDevicesChanged;
    }

    #region -- Public properties --

    private ICommand _roomTappedCommand;
    public ICommand RoomTappedCommand => _roomTappedCommand ??= SingleExecutionCommand.FromFunc<RoomBindableModel>(OnRoomTappedCommandAsync);

    private ICommand _accessorieTappedCommand;
    public ICommand AccessorieTappedCommand => _accessorieTappedCommand ??= new DelegateCommand<DeviceBindableModel>(async d => await OnAccessorieTappedCommandAsync(d));

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    private ObservableCollection<DeviceBindableModel> _favoriteAccessories = new();
    public ObservableCollection<DeviceBindableModel> FavoriteAccessories
    {
        get => _favoriteAccessories;
        set => SetProperty(ref _favoriteAccessories, value);
    }

    private ObservableCollection<RoomBindableModel> _rooms = new();
    public ObservableCollection<RoomBindableModel> Rooms
    {
        get => _rooms;
        set => SetProperty(ref _rooms, value);
    }

    #endregion

    #region -- Overrides --

    public override void OnAppearing()
    {
        base.OnAppearing();

        _isPageFocused = true;

        if (_needReloadDevice)
        {
            _needReloadDevice = false;

            LoadAllDevices();
        }
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();

        _isPageFocused = false;
    }

    public override void Destroy()
    {
        _roomsService.AllRoomsChanged -= OnAllRoomsChanged;
        _devicesService.AllDevicesChanged -= OnAllDevicesChanged;

        base.Destroy();
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            if (!IsDataLoading && DataState != EPageState.Complete)
            {
                DataState = EPageState.LoadingSkeleton;

                await LoadRoomsAndDevicesAndChangeStateAsync();
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private async void OnAllRoomsChanged(object sender, EventArgs e)
    {
        if (_roomsService.AllRooms is not null && _roomsService.AllRooms.Any())
        {
            LoadAllRooms();
        }
        else
        {
            DataState = EPageState.LoadingSkeleton;

            await LoadRoomsAndDevicesAndChangeStateAsync();
        }
    }

    private async void OnAllDevicesChanged(object sender, EventArgs e)
    {
        if (_devicesService.AllSupportedDevices is not null && _devicesService.AllSupportedDevices.Any())
        {
            if (_isPageFocused)
            {
                LoadAllDevices();
            }
            else
            {
                _needReloadDevice = true;
            }
        }
        else
        {
            DataState = EPageState.LoadingSkeleton;

            await LoadRoomsAndDevicesAndChangeStateAsync();
        }
    }

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.RepeatAsync(LoadRoomsAndDevicesAndChangeStateAsync, executionTime);
        }
    }

    private async Task<bool> LoadRoomsAndDevicesAndChangeStateAsync()
    {
        var isDataLoaded = false;

        if (IsInternetConnected)
        {
            isDataLoaded = await ReloadDevicesAsync();

            isDataLoaded &= await ReloadRoomsAsync();

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

        return isDataLoaded;
    }

    private async Task<bool> ReloadRoomsAsync()
    {
        var resultOfGettingRooms = await _roomsService.DownloadAllRoomsAsync();

        if (resultOfGettingRooms.IsSuccess)
        {
            LoadAllRooms();
        }
        else
        {
            Debug.WriteLine($"Can't download rooms: {resultOfGettingRooms.Message}");
        }

        return resultOfGettingRooms.IsSuccess;
    }

    private async Task<bool> ReloadDevicesAsync()
    {
        var aqaraDevicesResponse = await _devicesService.DownloadAllDevicesWithSubInfoAsync();

        if (aqaraDevicesResponse.IsSuccess)
        {
            LoadAllDevices();
        }
        else
        {
            Debug.WriteLine($"Can't download devices: {aqaraDevicesResponse.Message}");
        }

        return aqaraDevicesResponse.IsSuccess;
    }

    private void LoadAllDevices()
    {
        var devices = _devicesService.AllSupportedDevices.Where(device => device.IsFavorite);

        foreach (var device in devices)
        {
            device.TappedCommand = AccessorieTappedCommand;
        };

        FavoriteAccessories = new(devices);
    }

    private void LoadAllRooms()
    {
        var rooms = _roomsService.AllRooms;

        foreach (var room in rooms)
        {
            room.TappedCommand = RoomTappedCommand;
        }

        Rooms = new(rooms);
    }

    private async Task OnAccessorieTappedCommandAsync(DeviceBindableModel device)
    {
        if (device.DeviceType == EDeviceType.Switcher && device.Status != EDeviceStatus.Disconnected && !device.IsExecuting)
        {
            //Real device
            device.IsExecuting = true;

            var value = device.Status == EDeviceStatus.On ? "0" : "1";

            var updateResponse = await _devicesService.UpdateAttributeValueAsync(device.DeviceId, (device.EditableResourceId, value));

            if (updateResponse.IsSuccess)
            {
                device.Status = device.Status == EDeviceStatus.On ? EDeviceStatus.Off : EDeviceStatus.On;
            }
            else
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, updateResponse.Result?.Message },
                    { Constants.DialogsParameterKeys.DESCRIPTION, updateResponse.Result?.MsgDetails },
                });
            }

            device.IsExecuting = false;
        }
    }

    private Task OnRoomTappedCommandAsync(RoomBindableModel room)
    {
        return NavigationService.CreateBuilder()
            .AddSegment<RoomDetailsPageViewModel>(false)
            .AddParameter(KnownNavigationParameters.Animated, true)
            .AddParameter(nameof(Rooms), Rooms)
            .AddParameter(nameof(RoomBindableModel), room)
            .NavigateAsync().ContinueWith(x => DataState = EPageState.Complete);
    }

    #endregion
}

