namespace SmartMirror.Views.Dialogs;

public partial class EnterCodeDialog : Grid
{
    public EnterCodeDialog()
    {
        InitializeComponent();

        customNoBorderEntry.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(250), () =>
        {
            customNoBorderEntry.Focus();

            return false;
        });
    }
}