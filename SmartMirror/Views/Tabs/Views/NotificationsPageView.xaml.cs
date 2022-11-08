using System.ComponentModel;

namespace SmartMirror.Views.Tabs.Views;

public partial class NotificationsPageView : BaseContentView
{
	public NotificationsPageView()
	{
		InitializeComponent();

        //refreshView.PropertyChanged += OnRefreshViewPropertyChanged;
    }

    #region -- Private helpers --

    /*private void OnRefreshViewPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(refreshView.Height))
        {
            scrollViewNotifications.HeightRequest = refreshView.Height;

            scrollViewNotifications.LayoutTo(new Rect(0, 0, (int)refreshView.Width, (int)refreshView.Height), 100);
        }
    }*/

    #endregion
}