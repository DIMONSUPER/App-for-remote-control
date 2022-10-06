using SmartMirror.Enums;

namespace SmartMirror.ViewModels.Tabs;

public class NotificationsPageViewModel : BaseTabViewModel
{
    public NotificationsPageViewModel(
        INavigationService navigationService)
        : base(navigationService)
    {
        Title = "Notifications";
        DataState = EPageState.Complete;
    }
}

