using System.Collections.ObjectModel;
using SmartMirror.ViewModels.Tabs;

namespace SmartMirror.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    public MainPageViewModel(
        INavigationService navigationService)
        : base(navigationService)
    {
        Items = new()
        {
            new RoomsViewModel(),
            new NotificationsViewModel(),
            new CamerasViewModel(),
            new ScenariosViewModel(),
        };
    }

    #region -- Public properties --

    private ObservableCollection<BaseTabViewModel> _items;
    public ObservableCollection<BaseTabViewModel> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    #endregion
}
