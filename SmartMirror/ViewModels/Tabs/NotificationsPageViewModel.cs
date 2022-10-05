using SmartMirror.Enums;

namespace SmartMirror.ViewModels.Tabs;

public class NotificationsPageViewModel : BaseTabViewModel
{
    public NotificationsPageViewModel()
    {
        Title = "Notifications";
        DataState = EPageState.Complete;
    }
}

