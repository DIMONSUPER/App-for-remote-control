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

    #endregion

    #region -- Private helpers --

    private async void OnAllRoomsOrDevicesChanged(object sender, EventArgs e)
    {
        DataState = EPageState.LoadingSkeleton;

        var rooms = await _roomsService.GetAllRoomsAsync();

        foreach (var room in rooms)
        {
            room.SelectedCommand = RoomSelectedCommand;
        }

        Rooms = new(rooms);

        _selectedRoom ??= Rooms.FirstOrDefault();

        if (_selectedRoom is not null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SelectRoom(_selectedRoom);
            });
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

            var devices = await _devicesService.GetAllSupportedDevicesAsync();

            var roomDevices = devices.Where(x => x.PositionId == selectedRoom.Id && x.IsShownInRooms);

            foreach (var device in roomDevices)
            {
                device.TappedCommand = _accessorieTappedCommand;
            }

            if (roomDevices.Any())
            {
                DataState = EPageState.LoadingSkeleton;

                _ = Task.Run(() => MainThread.BeginInvokeOnMainThread(() =>
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

