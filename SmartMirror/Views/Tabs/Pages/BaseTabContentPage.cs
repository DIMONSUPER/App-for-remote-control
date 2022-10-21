namespace SmartMirror.Views.Tabs.Pages;

public class BaseTabContentPage : ContentPage
{
    public BaseTabContentPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
    }
}