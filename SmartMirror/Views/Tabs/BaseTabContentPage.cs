namespace SmartMirror.Views.Tabs;

public class BaseTabContentPage : ContentPage
{
    public BaseTabContentPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
    }

    #region -- Overrides --

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    #endregion
}

