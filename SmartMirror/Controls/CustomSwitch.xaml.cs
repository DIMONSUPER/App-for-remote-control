using System.Runtime.CompilerServices;

namespace SmartMirror.Controls;

public partial class CustomSwitch : ContentView
{
	public CustomSwitch()
	{
		InitializeComponent();
    }

    #region -- Public properties --

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
        propertyName: nameof(IsToggled),
        returnType: typeof(bool),
        declaringType: typeof(CustomSwitch));

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

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is nameof(IsToggled))
        {
            thumb.HorizontalOptions = IsToggled ? LayoutOptions.End : LayoutOptions.Start;
            thumb.Fill = IsToggled ? OnThumbColor : OffThumbColor;
            thumb.WidthRequest = IsToggled ? 24 : 21;
            thumb.HeightRequest = IsToggled ? 24 : 21;
            frame.BackgroundColor = IsToggled ? OnColor : OffColor;
        }
    }

    #endregion

    #region -- Private helpers --

    private void CustomSwitchToggled(System.Object sender, System.EventArgs e)
	{
        IsToggled = !IsToggled;
    }

    #endregion
}
