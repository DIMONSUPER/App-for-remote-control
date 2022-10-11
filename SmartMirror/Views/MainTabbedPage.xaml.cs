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
        var c = CurrentPage as NavigationPage;

        var result = false;

        if (c.CurrentPage == c.RootPage)
        {
            if (BindingContext is MainTabbedPageViewModel vm)
            {
                result = vm.OnBackButtonPressed();
            }
        }
        else if(c.CurrentPage.BindingContext is BaseViewModel vm)
        {
            result = vm.OnBackButtonPressed();
        }
        else
        {
            result = base.OnBackButtonPressed();
        }

        return result;
    }

    #endregion
}
