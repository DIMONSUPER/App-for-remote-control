using CommunityToolkit.Maui.Alerts;
using SmartMirror.Controls;
using SmartMirror.ViewModels;
using SmartMirror.ViewModels.Tabs;

namespace SmartMirror.Views;

public partial class MainTabbedPage : CustomTabbedPage
{
	public MainTabbedPage()
	{
		InitializeComponent();
	}

    #region -- Overrides --

    protected override bool OnBackButtonPressed()
    {
        var currentNavigationPage = CurrentPage as NavigationPage;

        var result = false;

        if (currentNavigationPage.CurrentPage == currentNavigationPage.RootPage && BindingContext is MainTabbedPageViewModel mainTabbedVm)
        {
            result = mainTabbedVm.OnBackButtonPressed();
        }
        else if (currentNavigationPage.CurrentPage.BindingContext is BaseViewModel baseVm)
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
