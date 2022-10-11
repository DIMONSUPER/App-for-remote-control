namespace SmartMirror.Views.Tabs;

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

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(150), () =>
        {
            scrollViewFavoriteAccessories.ScrollToAsync(0, 0, false);
            scrollViewRooms.ScrollToAsync(0, 0, false);

            return false;
        });
    }

    #endregion
}
