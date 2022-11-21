namespace SmartMirror.Views.Tabs.Pages;

public class CustomNavigationPage : NavigationPage
{
    #region -- Overrides --

    protected override bool OnBackButtonPressed()
    {
        return CurrentPage.SendBackButtonPressed();
    }

    #endregion
}

