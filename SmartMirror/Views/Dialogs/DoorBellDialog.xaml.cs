using SmartMirror.Controls;

namespace SmartMirror.Views.Dialogs;

public partial class DoorBellDialog : Grid
{
	public DoorBellDialog()
	{
        InitializeComponent();
	}

    private void OnCamerasPageUnloaded(object sender, EventArgs e)
    {
        video.Handler?.DisconnectHandler();
    }
}
