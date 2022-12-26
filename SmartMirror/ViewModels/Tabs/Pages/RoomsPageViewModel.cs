using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Rooms;
using SmartMirror.ViewModels.Tabs.Details;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using SmartMirror.Resources.Strings;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class RoomsPageViewModel : BaseTabViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IAqaraService _aqaraService;
    private readonly IDialogService _dialogService;
    private readonly IRoomsService _roomsService;
    private readonly IDevicesService _devicesService;

    public RoomsPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService,
        IAqaraService aqaraService,
        IRoomsService roomsService,
        IDialogService dialogService,
        IDevicesService devicesService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _aqaraService = aqaraService;
        _dialogService = dialogService;
        _roomsService = roomsService;
        _devicesService = devicesService;

        Title = "Rooms";

        DataState = EPageState.LoadingSkeleton;

        _roomsService.AllRoomsChanged += OnAllRoomsChanged;
        _devicesService.AllDevicesChanged += OnAllDevicesChanged;
    }

    #region -- Public properties --

    private ICommand _roomTappedCommand;
    public ICommand RoomTappedCommand => _roomTappedCommand ??= SingleExecutionCommand.FromFunc<RoomBindableModel>(OnRoomTappedCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

    private ICommand _accessorieTappedCommand;
    public ICommand AccessorieTappedCommand => _accessorieTappedCommand ??= SingleExecutionCommand.FromFunc<DeviceBindableModel>(OnAccessorieTappedCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

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

    public override void Destroy()
    {
        _roomsService.AllRoomsChanged -= OnAllRoomsChanged;
        _devicesService.AllDevicesChanged -= OnAllDevicesChanged;

        base.Destroy();
    }

    #endregion

    #region -- Private helpers --

    private async void OnAllRoomsChanged(object sender, EventArgs e)
    {
        await LoadRoomsAndDevicesAndChangeStateAsync();
    }

    private async void OnAllDevicesChanged(object sender, DeviceBindableModel device)
    {
        await LoadRoomsAndDevicesAndChangeStateAsync();
    }

    private async Task<bool> LoadRoomsAndDevicesAndChangeStateAsync()
    {
        bool isDataLoaded = false;

        if (IsInternetConnected)
        {
            await LoadAllDevicesAsync();
            await LoadAllRoomsAsync();

            if (IsInternetConnected)
            {
                isDataLoaded = Rooms.Any() || FavoriteAccessories.Any();

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

    private async Task LoadAllDevicesAsync()
    {
        var allDevices = await _devicesService.GetAllSupportedDevicesAsync();

        var favoriteDevices = allDevices.Where(device => device.IsFavorite);

        foreach (var device in favoriteDevices)
        {
            device.TappedCommand = AccessorieTappedCommand;
        };

        FavoriteAccessories = new(favoriteDevices);
    }

    private async Task LoadAllRoomsAsync()
    {
        var rooms = await _roomsService.GetAllRoomsAsync();

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
                    { Constants.DialogsParameterKeys.TITLE, updateResponse.Message },
                    { Constants.DialogsParameterKeys.DESCRIPTION, updateResponse.Result?.MsgDetails },
                });
            }

            device.IsExecuting = false;
        }
        else if (device.DeviceType == EDeviceType.DoorbellStream)
        {
            await _dialogService.ShowDialogAsync(nameof(DoorBellDialog), new DialogParameters()
            {
                { Constants.DialogsParameterKeys.ACCESSORY, device },
            });
        }
        else if (device.DeviceType == EDeviceType.DoorbellNoStream)
        {
            device.IsExecuting = true;

            var updateDevice = _mapperService.Map<DeviceBindableModel>(device);

            updateDevice.State = device.State == 0 ? 1 : 0;

            var updateResponse = await _devicesService.UpdateDeviceAsync(updateDevice, false);

            if (updateResponse.IsSuccess)
            {
                device.IsExecuting = false;

                device.State = device.State == 0 ? 1 : 0;
            }
            else
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, Strings.Error},
                    { Constants.DialogsParameterKeys.DESCRIPTION, updateResponse.Exception?.Message },
                });
            }
        }
    }

    private Task OnRoomTappedCommandAsync(RoomBindableModel room)
    {
        return NavigationService.CreateBuilder()
            .AddSegment<RoomDetailsPageViewModel>(false)
            .AddParameter(KnownNavigationParameters.Animated, true)
            .AddParameter(nameof(Rooms), Rooms)
            .AddParameter(nameof(RoomBindableModel), room)
            .AddParameter(nameof(AccessorieTappedCommand), AccessorieTappedCommand)
            .NavigateAsync().ContinueWith(x => DataState = EPageState.Complete);
    }

    #endregion
}

