namespace SmartMirror.Controls;

public partial class IconCheckBox : Border
{
    public IconCheckBox()
    {
        InitializeComponent();
    }

    #region -- Public properties --

    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(
        propertyName: nameof(IsChecked),
        returnType: typeof(bool),
        declaringType: typeof(IconCheckBox),
        defaultBindingMode: BindingMode.OneWay);

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public static readonly BindableProperty IconSizesProperty = BindableProperty.Create(
        propertyName: nameof(IconSizes),
        returnType: typeof(Size),
        defaultValue: new Size(30, 30),
        declaringType: typeof(IconCheckBox),
        defaultBindingMode: BindingMode.OneWay);

    public Size IconSizes
    {
        get => (Size)GetValue(IconSizesProperty);
        set => SetValue(IconSizesProperty, value);
    }

    public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
        propertyName: nameof(IconSource),
        returnType: typeof(string),
        defaultValue: "mdi_chevron_down",
        declaringType: typeof(IconCheckBox),
        defaultBindingMode: BindingMode.OneWay);

    public string IconSource
    {
        get => (string)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public static readonly BindableProperty CheckedIconSourceProperty = BindableProperty.Create(
        propertyName: nameof(CheckedIconSource),
        returnType: typeof(string),
        defaultValue: "mdi_chevron_up",
        declaringType: typeof(IconCheckBox),
        defaultBindingMode: BindingMode.OneWay);

    public string CheckedIconSource
    {
        get => (string)GetValue(CheckedIconSourceProperty);
        set => SetValue(CheckedIconSourceProperty, value);
    }

    #endregion

    #region -- Private helpers --

    private void OnTapped(Object sender, TappedEventArgs e) => IsChecked = !IsChecked;

    #endregion
}
