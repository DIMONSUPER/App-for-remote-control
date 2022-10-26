namespace SmartMirror.Views;

public class BaseContentPage : ContentPage
{
    public BaseContentPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
