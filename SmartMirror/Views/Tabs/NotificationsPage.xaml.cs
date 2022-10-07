namespace SmartMirror.Views.Tabs;

public partial class NotificationsPage : BaseTabContentPage
{
	public NotificationsPage()
	{
		InitializeComponent();
    }

    #region -- Private helpers --

    private void OnListViewHandlerChanged(object sender, EventArgs e)
	{
		if (sender is ListView listView && listView.Handler.PlatformView is Android.Widget.ListView nativeListView)
		{
#if ANDROID
			nativeListView.SetSelector(Android.Resource.Color.Transparent);
#endif
        }
	}

    #endregion
}
