namespace SmartMirror.Views.Tabs;

public partial class ScenariosPage : BaseTabContentPage
{
	public ScenariosPage()
	{
		InitializeComponent();
    }

    #region -- Overrides --
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(150), () =>
        {
            scrollViewCompleteState.ScrollToAsync(0, 0, false);
            scrollViewFavoriteScenarios.ScrollToAsync(0, 0, false);

            return false;
        });
    }

    #endregion
}
