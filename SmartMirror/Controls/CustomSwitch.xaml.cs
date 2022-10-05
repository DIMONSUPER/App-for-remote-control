using CommunityToolkit.Maui.Extensions;

namespace SmartMirror.Controls;

public partial class CustomSwitch : ContentView
{
	public CustomSwitch()
	{
		InitializeComponent();
    }

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
        propertyName: nameof(IsToggled),
        returnType: typeof(bool),
        propertyChanged: IsToggledPropertyChanged,
        declaringType: typeof(CustomSwitch));
    
    private static void IsToggledPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomSwitch customSwitch && newValue is bool)
        {
            customSwitch.thumb.HorizontalOptions = (bool)newValue ? LayoutOptions.End : LayoutOptions.Start;
            customSwitch.thumb.Fill = (bool)newValue ? customSwitch.OnThumbColor : customSwitch.OffThumbColor;
            customSwitch.thumb.WidthRequest = (bool)newValue ? 24 : 21;
            customSwitch.thumb.HeightRequest = (bool)newValue ? 24 : 21;
            customSwitch.frame.BackgroundColor = (bool)newValue ? customSwitch.OnColor : customSwitch.OffColor;
        }
    }

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public static readonly BindableProperty OnColorProperty = BindableProperty.Create(
        propertyName: nameof(OnColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomSwitch));

    public Color OnColor
    {
        get => (Color)GetValue(OnColorProperty);
        set => SetValue(OnColorProperty, value);
    }

    public static readonly BindableProperty OffColorProperty = BindableProperty.Create(
        propertyName: nameof(OffColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomSwitch));

    public Color OffColor
    {
        get => (Color)GetValue(OffColorProperty);
        set => SetValue(OffColorProperty, value);
    }

    public static readonly BindableProperty OnThumbColorProperty = BindableProperty.Create(
        propertyName: nameof(OnThumbColor),
        returnType: typeof(Color),
        defaultValue: Color.FromRgb(0, 0, 0),
        declaringType: typeof(CustomSwitch));

    public Color OnThumbColor
    {
        get => (Color)GetValue(OnThumbColorProperty);
        set => SetValue(OnThumbColorProperty, value);
    }

    public static readonly BindableProperty OffThumbColorProperty = BindableProperty.Create(
        propertyName: nameof(OffThumbColor),
        returnType: typeof(Color),
        defaultValue: Color.FromRgb(0, 0, 0),
        declaringType: typeof(CustomSwitch));

    public Color OffThumbColor
    {
        get => (Color)GetValue(OffThumbColorProperty);
        set => SetValue(OffThumbColorProperty, value);
    }

    void CustomSwitchToggled(System.Object sender, System.EventArgs e)
	{
        IsToggled = !IsToggled;
    }
}
