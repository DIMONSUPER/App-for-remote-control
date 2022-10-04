using SmartMirror.Enums;

namespace SmartMirror.ViewModels.Tabs;

public class ScenariosPageViewModel : BaseTabViewModel
{
    public ScenariosPageViewModel(
        INavigationService navigationService)
        : base(navigationService)
    {
        Title = "Scenarios";
        DataState = EPageState.Complete;
    }
}

