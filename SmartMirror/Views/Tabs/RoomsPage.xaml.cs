namespace SmartMirror.Views.Tabs;

public partial class RoomsPage : BaseTabContentPage
{
	public RoomsPage()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        return true;
        //return base.OnBackButtonPressed();
    }
}
