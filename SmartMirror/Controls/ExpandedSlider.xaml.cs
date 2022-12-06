using System.Windows.Input;

namespace SmartMirror.Controls;

public partial class ExpandedSlider : Border
{
    public ExpandedSlider()
    {
        InitializeComponent();
    }

    #region -- Public properties --

    public static readonly BindableProperty IsOpenedProperty = BindableProperty.Create(
        propertyName: nameof(IsOpened),
        returnType: typeof(bool),
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.OneWay);

    public bool IsOpened
    {
        get => (bool)GetValue(IsOpenedProperty);
        set => SetValue(IsOpenedProperty, value);
    }

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
        propertyName: nameof(Minimum),
        returnType: typeof(double),
        defaultValue: 0d,
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.TwoWay);

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
        propertyName: nameof(Maximum),
        returnType: typeof(double),
        defaultValue: 100d,
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.TwoWay);

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        propertyName: nameof(Value),
        returnType: typeof(double),
        defaultValue: 50d,
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.TwoWay);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty IconSizesProperty = BindableProperty.Create(
        propertyName: nameof(IconSizes),
        returnType: typeof(Size),
        defaultValue: new Size(18, 18),
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.OneWay);

    public static readonly BindableProperty StepProperty = BindableProperty.Create(
        propertyName: nameof(Step),
        returnType: typeof(int),
        declaringType: typeof(ExpandedSlider),
        defaultValue: 10,
        defaultBindingMode: BindingMode.OneWay);

    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public Size IconSizes
    {
        get => (Size)GetValue(IconSizesProperty);
        set => SetValue(IconSizesProperty, value);
    }

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        propertyName: nameof(IconColor),
        returnType: typeof(Color),
        defaultValue: Colors.Black,
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.OneWay);

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
        propertyName: nameof(IconSource),
        returnType: typeof(string),
        defaultValue: string.Empty,
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.OneWay);

    public string IconSource
    {
        get => (string)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public static readonly BindableProperty TextStyleProperty = BindableProperty.Create(
        propertyName: nameof(TextStyle),
        returnType: typeof(Style),
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.OneWay);

    public Style TextStyle
    {
        get => (Style)GetValue(TextStyleProperty);
        set => SetValue(TextStyleProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        propertyName: nameof(Text),
        returnType: typeof(string),
        defaultValue: string.Empty,
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.OneWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty OpenCommandProperty = BindableProperty.Create(
        propertyName: nameof(OpenCommand),
        returnType: typeof(ICommand),
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.OneWay);

    public ICommand OpenCommand
    {
        get => (ICommand)GetValue(OpenCommandProperty);
        set => SetValue(OpenCommandProperty, value);
    }

    public static readonly BindableProperty CloseCommandProperty = BindableProperty.Create(
        propertyName: nameof(CloseCommand),
        returnType: typeof(ICommand),
        declaringType: typeof(ExpandedSlider),
        defaultBindingMode: BindingMode.OneWay);

    public ICommand CloseCommand
    {
        get => (ICommand)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    #endregion

    #region -- Private helpers --

    private void OnOpenTapped(object sender, TappedEventArgs e)
    {
        if (!IsOpened)
        {
            IsOpened = true;
        }
    }

    private void OnCloseTapped(object sender, TappedEventArgs e)
    {
        IsOpened = false;
    }

    #endregion
}