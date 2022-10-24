using SmartMirror.Controls;

namespace SmartMirror.Views.Tabs.Pages;

public partial class CamerasPage : BaseTabContentPage
{
    public CamerasPage()
    {
        InitializeComponent();

        LazyView = lazyView;
    }
}
