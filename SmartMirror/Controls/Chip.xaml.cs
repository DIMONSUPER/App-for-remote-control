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

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        propertyName: nameof(FontFamily),
        returnType: typeof(string),
        defaultValue: "InterBold",
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        propertyName: nameof(FontSize),
        returnType: typeof(double),
        defaultValue: 21d,
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        propertyName: nameof(TextColor),
        returnType: typeof(Color),
        defaultValue: Color.FromArgb("#FFFFFF"),
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty CheckedTextColorProperty = BindableProperty.Create(
        propertyName: nameof(CheckedTextColor),
        returnType: typeof(Color),
        defaultValue: Color.FromArgb("#252525"),
        declaringType: typeof(Chip),
        defaultBindingMode: BindingMode.OneWay);

    public Color CheckedTextColor
    {
        get => (Color)GetValue(CheckedTextColorProperty);
        set => SetValue(CheckedTextColorProperty, value);
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
}