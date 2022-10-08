namespace SmartMirror.Views.Tabs;

public partial class CamerasPage : BaseTabContentPage
{
	public CamerasPage()
	{
		InitializeComponent();
	}

	#region -- Private helpers --

	private void OnCamerasPageUnloaded(object sender, EventArgs e)
	{
		videoPlayer.Handler?.DisconnectHandler();
	}

	private void OnCamerasPageDisappearing(object sender, EventArgs e)
	{
		videoPlayer.Stop();
	}

	private void BaseTabContentPage_Appearing(object sender, EventArgs e)
	{
		videoPlayer.Play();
	}

	#endregion
}
