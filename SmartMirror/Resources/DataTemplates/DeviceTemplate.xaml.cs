using SmartMirror.Controls;

namespace SmartMirror.Resources.DataTemplates;

public partial class DeviceTemplate : Grid
{
	public DeviceTemplate()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty InnerPaddingProperty = BindableProperty.Create(
        propertyName: nameof(InnerPadding),
        returnType: typeof(Thickness),
        declaringType: typeof(DeviceTemplate),
        defaultValue: new Thickness(24));

    public Thickness InnerPadding
    {
        get => (Thickness)GetValue(InnerPaddingProperty);
        set => SetValue(InnerPaddingProperty, value);
    }

    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        propertyName: nameof(IconSize),
        returnType: typeof(double),
        declaringType: typeof(DeviceTemplate),
        defaultValue: 84.0);

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly BindableProperty ActivityIndicatorSizeProperty = BindableProperty.Create(
        propertyName: nameof(ActivityIndicatorSize),
        returnType: typeof(double),
        declaringType: typeof(DeviceTemplate),
        defaultValue: 35.0);

    public double ActivityIndicatorSize
    {
        get => (double)GetValue(ActivityIndicatorSizeProperty);
        set => SetValue(ActivityIndicatorSizeProperty, value);
    }

    public static readonly BindableProperty IsRoomNameVisibleProperty = BindableProperty.Create(
        propertyName: nameof(IsRoomNameVisible),
        returnType: typeof(bool),
        declaringType: typeof(DeviceTemplate),
        defaultValue: true);

    public bool IsRoomNameVisible
    {
        get => (bool)GetValue(IsRoomNameVisibleProperty);
        set => SetValue(IsRoomNameVisibleProperty, value);
    }

    public static readonly BindableProperty DescriptionLabelStyleProperty = BindableProperty.Create(
        propertyName: nameof(DescriptionLabelStyle),
        returnType: typeof(Style),
        declaringType: typeof(DeviceTemplate));

    public Style DescriptionLabelStyle
    {
        get => (Style)GetValue(DescriptionLabelStyleProperty);
        set => SetValue(DescriptionLabelStyleProperty, value);
    }

    #endregion
}
