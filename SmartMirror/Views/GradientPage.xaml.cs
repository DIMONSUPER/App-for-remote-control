using SmartMirror.Effects;

namespace SmartMirror.Views;

public partial class GradientPage : ContentPage
{
	public GradientPage()
	{
		InitializeComponent();
	}

	private void slider1_ValueChanged(object sender, ValueChangedEventArgs e)
	{
		var value = e.NewValue;
        var color = Color.FromRgba(value, value, value, 1);

		LinearGradientEffect.SetStartColor(grid1, color);
		LinearGradientEffect.SetEndColor(grid1, Color.FromArgb("#202020"));
    }

	private void slider2_ValueChanged(object sender, ValueChangedEventArgs e)
	{
        var value = e.NewValue;
		var color = Color.FromRgba(value, value, value, 1);

		GradientStopCollection gradientStops = new GradientStopCollection();
		gradientStops.Add(new GradientStop(color, 0));
		gradientStops.Add(new GradientStop(Color.FromArgb("#202020"), 1));

		grid2.Background = new LinearGradientBrush(gradientStops);
	}
}