using System.Collections.ObjectModel;
using SmartMirror.Services.Mock;
using SmartMirror.ViewModels.Tabs;

namespace SmartMirror.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    private IMockService _mockService;
    public MainPageViewModel(
        INavigationService navigationService,
        IMockService mockService)
        : base(navigationService)
    {
        _mockService = mockService;

        Items = new()
        {
            new RoomsViewModel(_mockService),
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
