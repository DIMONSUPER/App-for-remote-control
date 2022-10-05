using System;
using SmartMirror.Enums;

namespace SmartMirror.ViewModels.Tabs;

public class CamerasPageViewModel : BaseTabViewModel
{
    public CamerasPageViewModel(
        INavigationService navigationService)
        : base(navigationService)
    {
        Title = "Cameras";
        DataState = EPageState.Complete;
    }
}
