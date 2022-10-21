namespace SmartMirror.Views.Tabs.Pages;

public partial class CamerasPage : BaseTabContentPage
{
	public CamerasPage()
	{
		InitializeComponent();
    }

    #region -- Overrides --

    protected override void OnAppearing()
	{
		base.OnAppearing();

		lazyView.TryLoadView();


        (Content as IView).InvalidateMeasure();
    }

	#endregion
}
