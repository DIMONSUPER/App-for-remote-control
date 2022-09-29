using System.Collections.ObjectModel;

namespace SmartMirror.ViewModels.Tabs;

public class RoomsViewModel : BaseTabViewModel
{
    public RoomsViewModel()
    {
        Title = "Rooms";
    }

    #region -- Public properties --

    private ObservableCollection<TemporaryModel> _favoriteAccessories;
    public ObservableCollection<TemporaryModel> FavoriteAccessories
    {
        get => _favoriteAccessories;
        set => SetProperty(ref _favoriteAccessories, value);
    }

    private ObservableCollection<TemporaryModel> _rooms;
    public ObservableCollection<TemporaryModel> Rooms
    {
        get => _rooms;
        set => SetProperty(ref _rooms, value);
    }

    #endregion

    #region -- Overrides --

    public override void Initialize(INavigationParameters parameters)
    {
        base.Initialize(parameters);

        FavoriteAccessories = new()
        {
            new() { Name = "Garage Door", Type = "GarageDoor", IsEnabled = true },
            new() { Name = "Front Door", Type = "FrontDoor", IsEnabled = true },
            new() { Name = "Fan", Type = "Fan", IsEnabled = true, RoomName = "Living Room", Description = "68%" },
        };

        Rooms = new()
        {
            new() { Name = "Dining Room", Description = "5 Accessories" },
            new() { Name = "Downstairs", Description = "4 Accessories" },
            new() { Name = "Front Door", Description = "2 Accessories" },
            new() { Name = "Garage", Description = "12 Accessories" },
        };
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

