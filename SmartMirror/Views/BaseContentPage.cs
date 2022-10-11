using SmartMirror.ViewModels;

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
        bool result;

        if (BindingContext is BaseViewModel viewModel)
        {
            result = true;
            viewModel.OnBackButtonPressed();
        }
        else
        {
            result = base.OnBackButtonPressed();
        }

        return result;
    }

    #endregion
}

