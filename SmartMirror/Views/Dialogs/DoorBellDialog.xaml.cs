using SmartMirror.Controls;

namespace SmartMirror.Views.Dialogs;

public partial class DoorBellDialog : BaseDialogView
{
	public DoorBellDialog()
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
