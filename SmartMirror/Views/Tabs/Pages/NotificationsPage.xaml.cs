namespace SmartMirror.Views.Tabs.Pages;

public partial class NotificationsPage : BaseTabContentPage
{
	public NotificationsPage()
	{
		InitializeComponent();
    }

    #region -- Overrides --

    protected override void OnAppearing()
    {
        base.OnAppearing();

        lazyView.TryLoadView();

        if (lazyView.Content is IPageLifecycleAware content)
        {
            content.OnAppearing();
        }
    }

    #endregion
}
