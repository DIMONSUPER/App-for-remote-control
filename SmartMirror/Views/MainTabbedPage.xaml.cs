using SmartMirror.Controls;

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
        return true;
    }

    #endregion
}
