using System.Collections.ObjectModel;
using SmartMirror.Models;
using SmartMirror.Services.Mock;
using Device = SmartMirror.Models.Device;

namespace SmartMirror.ViewModels.Tabs;

public class RoomsViewModel : BaseTabViewModel
{
    private readonly ISmartHomeMockService _smartHomeMockService;

    public RoomsViewModel(ISmartHomeMockService smartHomeMockService)
    {
        Title = "Rooms";
        _smartHomeMockService = smartHomeMockService;
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

        FavoriteAccessories = new(_smartHomeMockService.GetDevices());
        Rooms = new(_smartHomeMockService.GetRooms());
    }

    #endregion
}

