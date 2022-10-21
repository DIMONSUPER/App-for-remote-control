namespace SmartMirror.Views.Tabs.Pages;

public partial class RoomsPage : BaseTabContentPage
{
	public RoomsPage()
	{
		InitializeComponent();
    }

    #region -- Overrides --

    protected override void OnAppearing()
    {
        base.OnAppearing();

        lazyView.LoadView();

        if (lazyView.Content is IPageLifecycleAware content)
        {
            content.OnAppearing();
        }
    }

    #endregion
}
