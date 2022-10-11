using System.ComponentModel;

namespace SmartMirror.Views.Tabs;

public partial class NotificationsPage : BaseTabContentPage
{
	public NotificationsPage()
	{
		InitializeComponent();

        refreshView.PropertyChanged += OnRefreshViewPropertyChanged;
    }

    #region -- Overrides --

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(150), () =>
        {
            scrollViewNotifications.ScrollToAsync(0, 0, false);

            return false;
        });
    }

    #endregion

    #region -- Private helpers --

    private void OnRefreshViewPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(refreshView.Height))
        {
            scrollViewNotifications.HeightRequest = refreshView.Height;
            scrollViewNotifications.LayoutTo(new Rect(0, 0, (int)refreshView.Width, (int)refreshView.Height), 100);
        }
    }

    #endregion
}
