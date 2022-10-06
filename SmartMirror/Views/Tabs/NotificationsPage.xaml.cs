namespace SmartMirror.Views.Tabs;

public partial class NotificationsPage : BaseTabContentPage
{
	public NotificationsPage()
	{
		InitializeComponent();
    }

	private void ListView_HandlerChanged(object sender, EventArgs e)
	{
		if (sender is ListView listView)
		{
#if ANDROID
			var nativeListView = listView.Handler.PlatformView as Android.Widget.ListView;

			nativeListView.SetSelector(Android.Resource.Color.Transparent);
#endif
        }
	}
}
