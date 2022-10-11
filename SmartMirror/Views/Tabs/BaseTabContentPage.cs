using CommunityToolkit.Maui.Alerts;
using SmartMirror.ViewModels.Tabs;

namespace SmartMirror.Views.Tabs;

public class BaseTabContentPage : ContentPage
{
    public BaseTabContentPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
    }
}

