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
        var result = false;

        if (BindingContext is ViewModels.BaseViewModel baseVm)
        {
            result = baseVm.OnBackButtonPressed();
        }
        else
        {
            result = base.OnBackButtonPressed();
        }

        return result;
    }

    #endregion
}
