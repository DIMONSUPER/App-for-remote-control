namespace SmartMirror.Controls;

public partial class Chip : Border
{
	public Chip()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(
        propertyName: nameof(IsChecked),
        returnType: typeof(bool),
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public static readonly BindableProperty IconSizesProperty = BindableProperty.Create(
        propertyName: nameof(IconSizes),
        returnType: typeof(Size),
        defaultValue: new Size(19, 19),
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public Size IconSizes
    {
        get => (Size)GetValue(IconSizesProperty);
        set => SetValue(IconSizesProperty, value);
    }

    public static readonly BindableProperty TextStyleProperty = BindableProperty.Create(
        propertyName: nameof(TextStyle),
        returnType: typeof(Style),
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public Style TextStyle
    {
        get => (Style)GetValue(TextStyleProperty);
        set => SetValue(TextStyleProperty, value);
    }

    public static readonly BindableProperty CheckedTextStyleProperty = BindableProperty.Create(
        propertyName: nameof(CheckedTextStyle),
        returnType: typeof(Style),
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public Style CheckedTextStyle
    {
        get => (Style)GetValue(CheckedTextStyleProperty);
        set => SetValue(CheckedTextStyleProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        propertyName: nameof(Text),
        returnType: typeof(string),
        defaultValue: string.Empty,
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        propertyName: nameof(IconColor),
        returnType: typeof(Color),
        defaultValue: Colors.Black,
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
        propertyName: nameof(IconSource),
        returnType: typeof(string),
        defaultValue: "ci_close_big",
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public string IconSource
    {
        get => (string)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public static readonly BindableProperty CheckedBackgroundColorProperty = BindableProperty.Create(
        propertyName: nameof(CheckedBackgroundColor),
        returnType: typeof(Color),
        defaultValue: Colors.Black,
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public Color CheckedBackgroundColor
    {
        get => (Color)GetValue(CheckedBackgroundColorProperty);
        set => SetValue(CheckedBackgroundColorProperty, value);
    }

    #endregion

    #region -- Private helpers --

    private void OnTapped(Object sender, TappedEventArgs e)
    {
        IsChecked = !IsChecked;
    }

    #endregion
}