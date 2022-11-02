using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Rooms;
using SmartMirror.ViewModels.Tabs.Pages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs.Details;

public class RoomDetailsPageViewModel : BaseViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IDevicesService _devicesService;
    private readonly IRoomsService _roomsService;

    private RoomBindableModel _selectedRoom;

    public RoomDetailsPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService,
        IDevicesService devicesService,
        IRoomsService roomsService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _devicesService = devicesService;
        _roomsService = roomsService;

        _roomsService.AllRoomsChanged += OnAllRoomsOrDevicesChanged;
        _devicesService.AllDevicesChanged += OnAllRoomsOrDevicesChanged;
        DataState = EPageState.LoadingSkeleton;
    }

    #region -- Public properties --

    private ICommand _backArrowTappedCommand;
    public ICommand BackArrowTappedCommand => _backArrowTappedCommand ??= SingleExecutionCommand.FromFunc(OnBackArrowTappedCommandAsync);

    private ICommand _roomSelectedCommand;
    public ICommand RoomSelectedCommand => _roomSelectedCommand ??= SingleExecutionCommand.FromFunc<RoomBindableModel>(OnRoomSelectedCommandAsync, delayMillisec: 0);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

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
        _roomsService.AllRoomsChanged -= OnAllRoomsOrDevicesChanged;
        _devicesService.AllDevicesChanged -= OnAllRoomsOrDevicesChanged;

        base.Destroy();
    }

    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        base.OnNavigatedTo(parameters);

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
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            DataState = EPageState.LoadingSkeleton;

            var devicesResponse = await _devicesService.DownloadAllDevicesWithSubInfoAsync();

            var downloadRoomsResponse = await _roomsService.DownloadAllRoomsAsync();

            if (downloadRoomsResponse.IsSuccess)
            {
                Rooms = new(_roomsService.AllRooms);
            }

            if (devicesResponse.IsSuccess)
            {
                SelectRoom(_selectedRoom);
            }
            else
            {
                //TODO: devices are not updated
            }
        }
        else
        {
            DataState = EPageState.Complete;
            RoomDeviceState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private void InitialLoadRoomsAsync(IEnumerable<RoomBindableModel> rooms, RoomBindableModel selectedRoom)
    {

    }

    private void OnAllRoomsOrDevicesChanged(object sender, EventArgs e)
    {
        if (_selectedRoom is not null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DataState = EPageState.LoadingSkeleton;

                SelectRoom(_selectedRoom);
            });
        }
    }

    private Task OnBackArrowTappedCommandAsync()
    {
        return NavigationService.GoBackAsync();
    }

    private void SelectRoom(RoomBindableModel selectedRoom)
    {
        if (Rooms?.Count > 0)
        {
            _selectedRoom = selectedRoom;

            foreach (var room in Rooms)
            {
                room.IsSelected = room.Id == selectedRoom.Id;
            }

            var roomDevices = _devicesService.AllSupportedDevices.Where(x => x.PositionId == selectedRoom.Id);

            if (roomDevices.Any())
            {
                DataState = EPageState.LoadingSkeleton;

                Task.Run(() => MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectedRoomDevices = new(roomDevices);

                    DataState = EPageState.Complete;
                    RoomDeviceState = EPageState.Complete;
                }));
            }
            else
            {
                SelectedRoomDevices = new();

                DataState = EPageState.Complete;
                RoomDeviceState = IsInternetConnected
                    ? EPageState.Empty
                    : EPageState.NoInternet;
            }
        }
    }

    private Task OnRoomSelectedCommandAsync(RoomBindableModel room)
    {
        SelectRoom(room);

        return Task.CompletedTask;
    }

    private Task OnTryAgainCommandAsync()
    {
        DataState = EPageState.Complete;
        RoomDeviceState = EPageState.NoInternetLoader;

        SelectRoom(_selectedRoom);

        return Task.CompletedTask;
    }

    #endregion
}

