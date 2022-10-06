namespace SmartMirror.Views;

public partial class SplashScreenPage : BaseContentPage
{
	public SplashScreenPage()
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
