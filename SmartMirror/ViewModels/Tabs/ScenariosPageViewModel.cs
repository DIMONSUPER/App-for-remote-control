using SmartMirror.Enums;

namespace SmartMirror.ViewModels.Tabs;

public class ScenariosPageViewModel : BaseTabViewModel
{
    public ScenariosPageViewModel()
    {
        Title = "Scenarios";
        DataState = EPageState.Complete;
    }
}

