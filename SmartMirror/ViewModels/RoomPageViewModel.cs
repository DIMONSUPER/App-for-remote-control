using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using SmartMirror.ViewModels.Tabs;
using Device = SmartMirror.Models.Device;

namespace SmartMirror.ViewModels;

public class RoomPageViewModel : BaseViewModel
{
    private readonly IMapperService _mapperService;
    private readonly ISmartHomeMockService _smartHomeMockService;

    public RoomPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService,
        ISmartHomeMockService smartHomeMockService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _smartHomeMockService = smartHomeMockService;
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

    private ObservableCollection<Device> _selectedRoomDevices;
    public ObservableCollection<Device> SelectedRoomDevices
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

    private void SelectRoom(RoomBindableModel selectedRoom)
    {
        if (Rooms is not null && Rooms.Any())
        {
            foreach (var room in Rooms)
            {
                room.IsSelected = room == selectedRoom;
                SelectedRoomDevices = room.Devices;
            }
        }
    }

    private Task OnRoomSelectedCommandAsync(RoomBindableModel room)
    {
        SelectRoom(room);

        return Task.CompletedTask;
    }

    #endregion
}

