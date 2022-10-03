using System.Collections.ObjectModel;
using SmartMirror.Services.Mock;
using SmartMirror.ViewModels.Tabs;

namespace SmartMirror.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    private readonly ISmartHomeMockService _smartHomeMockService;

    public MainPageViewModel(
        INavigationService navigationService,
        ISmartHomeMockService smartHomeMockService)
        : base(navigationService)
    {
        _smartHomeMockService = smartHomeMockService;

        Items = new()
        {
            new RoomsViewModel(_smartHomeMockService),
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
