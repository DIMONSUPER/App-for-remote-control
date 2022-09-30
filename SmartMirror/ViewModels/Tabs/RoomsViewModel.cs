using System.Collections.ObjectModel;
using SmartMirror.Models;
using SmartMirror.Services.Mock;
using Device = SmartMirror.Models.Device;

namespace SmartMirror.ViewModels.Tabs;

public class RoomsViewModel : BaseTabViewModel
{
    private readonly IMockService _mokcService;

    public RoomsViewModel(IMockService mockService)
    {
        Title = "Rooms";
        _mokcService = mockService;
    }

    #region -- Public properties --

    private ObservableCollection<Device> _favoriteAccessories;
    public ObservableCollection<Device> FavoriteAccessories
    {
        get => _favoriteAccessories;
        set => SetProperty(ref _favoriteAccessories, value);
    }

    private ObservableCollection<Room> _rooms;
    public ObservableCollection<Room> Rooms
    {
        get => _rooms;
        set => SetProperty(ref _rooms, value);
    }

    #endregion

    #region -- Overrides --

    public override void Initialize(INavigationParameters parameters)
    {
        base.Initialize(parameters);

        FavoriteAccessories = new(_mokcService.GetDevices());
        Rooms = new(_mokcService.GetRooms());
    }

    #endregion
}

//TODO: replace this with mocked models
public class TemporaryModel : BindableBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string RoomName { get; set; }

    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
}

