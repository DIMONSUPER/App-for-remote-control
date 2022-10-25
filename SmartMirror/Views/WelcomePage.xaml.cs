using SmartMirror.ViewModels;

namespace SmartMirror.Views;

public partial class WelcomePage : BaseContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
    }

    #region -- Public properties --

    public static readonly BindableProperty ScreenStreamProperty = BindableProperty.Create(
            propertyName: nameof(ScreenStream),
            returnType: typeof(Stream),
            declaringType: typeof(WelcomePage),
            defaultValue: default(Stream));

    public Stream ScreenStream
    {
        get => (Stream)GetValue(ScreenStreamProperty);
        set => SetValue(ScreenStreamProperty, value);
    }

    #endregion

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
	{
        var screen = await page.CaptureAsync();

        if (BindingContext is WelcomePageViewModel viewModel)
        {
            viewModel.ScreenStream = await screen.OpenReadAsync();
        }

        image.Source = ImageSource.FromStream(ddd);
    }

    private Stream ddd()
    {
        Stream stream = null;

        if (BindingContext is WelcomePageViewModel viewModel)
        {
            stream = viewModel.ScreenStream;
        }

        return stream;
    }
}