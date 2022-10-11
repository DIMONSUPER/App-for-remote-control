namespace SmartMirror.Views.Tabs;

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

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(150), () =>
        {
            collectionViewCameras.ScrollTo(0, -1, ScrollToPosition.Start, false);

            return false;
        });
    }

    #endregion
}
