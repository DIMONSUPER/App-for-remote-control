using System;
using SmartMirror.Enums;

namespace SmartMirror.ViewModels.Tabs
{
    public class CamerasPageViewModel : BaseTabViewModel
    {
        public CamerasPageViewModel()
        {
            Title = "Cameras";
            DataState = EPageState.Complete;
        }
    }
}
