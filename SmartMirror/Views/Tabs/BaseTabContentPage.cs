using CommunityToolkit.Maui.Alerts;
using SmartMirror.ViewModels.Tabs;

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
        bool result = true;

        if (BindingContext is BaseTabViewModel viewModel)
        {
            //viewModel.OnBackButtonPressed();
        }

        return result;
    }

    #endregion
}

