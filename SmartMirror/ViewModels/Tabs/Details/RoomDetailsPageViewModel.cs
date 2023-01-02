using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Rooms;
using SmartMirror.ViewModels.Tabs.Pages;
using SmartMirror.Views.Dialogs;
using SmartMirror.Resources.Strings;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartMirror.Extensions;

namespace SmartMirror.ViewModels.Tabs.Details;

public class RoomDetailsPageViewModel : BaseViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IDevicesService _devicesService;
    private readonly IRoomsService _roomsService;
    private readonly IDialogService _dialogService;

    private RoomBindableModel _selectedRoom;
    private ICommand _accessorieTappedCommand;

    public RoomDetailsPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService,
        IDevicesService devicesService,
        IRoomsService roomsService,
        IDialogService dialogService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _devicesService = devicesService;
        _roomsService = roomsService;
        _dialogService = dialogService;

        _roomsService.AllRoomsChanged += OnAllRoomsChanged;
        _devicesService.AllDevicesChanged += OnAllDevicesChanged;

        DataState = EPageState.LoadingSkeleton;
    }

    #region -- Public properties --

    private ICommand _backArrowTappedCommand;
    public ICommand BackArrowTappedCommand => _backArrowTappedCommand ??= SingleExecutionCommand.FromFunc(OnBackArrowTappedCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

    private ICommand _roomSelectedCommand;
    public ICommand RoomSelectedCommand => _roomSelectedCommand ??= SingleExecutionCommand.FromFunc<RoomBindableModel>(OnRoomSelectedCommandAsync, delayMillisec: 0);

    private ObservableCollection<RoomBindableModel> _rooms;
    public ObservableCollection<RoomBindableModel> Rooms
    {
        get => _rooms;
        set => SetProperty(ref _rooms, value);
    }

    private ObservableCollection<DeviceBindableModel> _selectedRoomDevices;
    public ObservableCollection<DeviceBindableModel> SelectedRoomDevices
    {
        get => _selectedRoomDevices;
        set => SetProperty(ref _selectedRoomDevices, value);
    }

    private EPageState _roomDevicesState;
    public EPageState RoomDeviceState
    {
        get => _roomDevicesState;
        set => SetProperty(ref _roomDevicesState, value);
    }

    #endregion

    #region -- Overrides --

    public override void Destroy()
    {
        _roomsService.AllRoomsChanged -= OnAllRoomsChanged;
        _devicesService.AllDevicesChanged -= OnAllDevicesChanged;

        base.Destroy();
    }

    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        base.OnNavigatedTo(parameters);

        if (parameters.TryGetValue(nameof(RoomsPageViewModel.AccessorieTappedCommand), out ICommand command))
        {
            _accessorieTappedCommand = command;
        }

        if (parameters.TryGetValue(nameof(RoomsPageViewModel.Rooms), out IEnumerable<RoomBindableModel> rooms))
        {
            RoomBindableModel selectedRoom;

            if (parameters.TryGetValue(nameof(RoomBindableModel), out RoomBindableModel roomBindable))
            {
                selectedRoom = roomBindable;
            }
            else
            {
                selectedRoom = Rooms?.FirstOrDefault();
            }

            foreach (var room in rooms)
            {
                room.SelectedCommand = RoomSelectedCommand;
            }

            Rooms = new(rooms);

            SelectRoom(selectedRoom);
        }
        else
        {
            DataState = EPageState.Complete;
        }
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (IsInternetConnected)
        {
            DataState = EPageState.LoadingSkeleton;

            await LoadRoomsAndChangeStateAsync();
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
        await LoadRoomsAndChangeStateAsync();
    }

    private async void OnAllDevicesChanged(object sender, DeviceBindableModel device)
    {
        if (device is null)
        {
            await LoadRoomsAndChangeStateAsync();
        }
        else
        {
            SelectedRoomDevices.Update(device);

            RoomDeviceState = SelectedRoomDevices.Count == 0
                ? EPageState.Empty
                : EPageState.Complete;
        }
    }

    private async Task LoadRoomsAndChangeStateAsync()
    {
        var rooms = await _roomsService.GetAllRoomsAsync();

        foreach (var room in rooms)
        {
            room.SelectedCommand = RoomSelectedCommand;
        }

        Rooms = new(rooms);

        _selectedRoom ??= Rooms.FirstOrDefault();

        if (_selectedRoom is not null)
        {
            MainThread.BeginInvokeOnMainThread(() => SelectRoom(_selectedRoom));
        }
    }

    private Task OnBackArrowTappedCommandAsync()
    {
        return NavigationService.GoBackAsync();
    }

    private async void SelectRoom(RoomBindableModel selectedRoom)
    {
        if (Rooms?.Count > 0)
        {
            _selectedRoom = selectedRoom;

            foreach (var room in Rooms)
            {
                room.IsSelected = room.Id == selectedRoom.Id;
            }

            if (selectedRoom.DevicesCount == 0)
            {
                SelectedRoomDevices = new();

                DataState = EPageState.Complete;
                RoomDeviceState = EPageState.Empty;
            }
            else
            {
                await LoadDevicesForRoomAsync(selectedRoom.Id);

                _ = Task.Run(() => MainThread.BeginInvokeOnMainThread(() =>
                {
                    DataState = EPageState.Complete;
                    RoomDeviceState = EPageState.Complete;
                }));
            }
        }
    }

    private async Task LoadDevicesForRoomAsync(string roomId)
    {
        var devices = await _devicesService.GetAllSupportedDevicesAsync();

        var roomDevices = devices.Where(x => x.PositionId == roomId && x.IsShownInRooms);

        foreach (var device in roomDevices)
        {
            device.TappedCommand = _accessorieTappedCommand;
        }

        SelectedRoomDevices = new(roomDevices);
    }

    private Task OnRoomSelectedCommandAsync(RoomBindableModel room)
    {
        DataState = EPageState.LoadingSkeleton;

        SelectRoom(room);

        return Task.CompletedTask;
    }

    #endregion
}

