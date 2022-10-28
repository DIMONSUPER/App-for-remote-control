namespace SmartMirror.Views.Tabs.Views;

public partial class CamerasPageView : BaseContentView
{
	public CamerasPageView()
	{
		InitializeComponent();
	}

	#region -- Private helpers --
	
	private void OnCamerasPageUnloaded(object sender, EventArgs e)
	{
		video.Handler?.DisconnectHandler();
	} 

	#endregion
}