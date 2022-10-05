namespace SmartMirror.Views;

public class BaseContentPage : ContentPage
{
    public BaseContentPage()
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

