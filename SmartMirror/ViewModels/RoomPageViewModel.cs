using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels;

public class RoomPageViewModel : BaseViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IDevicesService _devicesService;

    private RoomBindableModel _selectedRoom = new();

    public RoomPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService,
        IDevicesService devicesService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _devicesService = devicesService;
    }

    #region -- Public properties --

    private ICommand _backArrowTappedCommand;
    public ICommand BackArrowTappedCommand => _backArrowTappedCommand ??= SingleExecutionCommand.FromFunc(OnBackArrowTappedCommandAsync);

    private ICommand _roomSelectedCommand;
    public ICommand RoomSelectedCommand => _roomSelectedCommand ??= SingleExecutionCommand.FromFunc<RoomBindableModel>(OnRoomSelectedCommandAsync, delayMillisec: 0);

    private ObservableCollection<RoomBindableModel> _rooms;
    public ObservableCollection<RoomBindableModel> Rooms
    {
        get => _rooms;
        set => SetProperty(ref _rooms, value);
    }

    private ObservableCollection<DeviceModel> _selectedRoomDevices;
    public ObservableCollection<DeviceModel> SelectedRoomDevices
    {
        get => _selectedRoomDevices;
        set => SetProperty(ref _selectedRoomDevices, value);
    }

    #endregion

    #region -- Overrides --

    public override void Initialize(INavigationParameters parameters)
    {
        base.Initialize(parameters);

        if (parameters.TryGetValue(nameof(RoomsPageViewModel.Rooms), out IEnumerable<RoomBindableModel> rooms))
        {
            foreach (var room in rooms)
            {
                room.SelectedCommand = RoomSelectedCommand;
            }

            Rooms = new(rooms);
        }

        if (parameters.TryGetValue(nameof(RoomBindableModel), out RoomBindableModel selectedRoom))
        {
            SelectRoom(selectedRoom);
        }
    }

    #endregion

    #region -- Private helpers --

    private Task OnBackArrowTappedCommandAsync()
    {
        return NavigationService.GoBackAsync();
    }

    private async void SelectRoom(RoomBindableModel selectedRoom)
    {
        if (Rooms?.Count > 0)
        {
            DataState = EPageState.Loading;

            _selectedRoom.IsSelected = false;
            selectedRoom.IsSelected = true;

            var resultOfGettingDevices = await _devicesService.GetDevicesByPositionAsync(selectedRoom.Id);

            if (resultOfGettingDevices.IsSuccess)
            {
                SelectedRoomDevices = new(resultOfGettingDevices.Result);
            }
            else
            {
                SelectedRoomDevices = new();
            }

            DataState = SelectedRoomDevices.Count == 0
                ? EPageState.Empty
                : EPageState.Complete;

            _selectedRoom = selectedRoom;
        }
    }

    private Task OnRoomSelectedCommandAsync(RoomBindableModel room)
    {
        DataState = EPageState.Loading;

        SelectRoom(room);

        return Task.CompletedTask;
    }

    #endregion
}

