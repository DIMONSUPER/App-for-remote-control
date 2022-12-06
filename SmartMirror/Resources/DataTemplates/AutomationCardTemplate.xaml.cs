namespace SmartMirror.Resources.DataTemplates;

public partial class AutomationCardTemplate : Grid
{
    public AutomationCardTemplate()
    {
        InitializeComponent();
    }

    #region -- Public properties --

    public static readonly BindableProperty InnerPaddingProperty = BindableProperty.Create(
        propertyName: nameof(InnerPadding),
        returnType: typeof(Thickness),
        declaringType: typeof(AutomationCardTemplate),
        defaultValue: new Thickness(24));

    public Thickness InnerPadding
    {
        get => (Thickness)GetValue(InnerPaddingProperty);
        set => SetValue(InnerPaddingProperty, value);
    }

    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        propertyName: nameof(IconSize),
        returnType: typeof(double),
        declaringType: typeof(AutomationCardTemplate),
        defaultValue: 84.0);

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    #endregion
}
