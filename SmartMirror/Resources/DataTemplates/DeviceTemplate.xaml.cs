using SmartMirror.Controls;
using SmartMirror.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SmartMirror.Models.BindableModels;
using System;

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

    public static readonly BindableProperty DeviceStatusProperty = BindableProperty.Create(
        propertyName: nameof(DeviceStatus),
        returnType: typeof(EDeviceStatus),
        declaringType: typeof(DeviceTemplate),
        defaultValue: EDeviceStatus.Disconnected);

    public EDeviceStatus DeviceStatus
    {
        get => (EDeviceStatus)GetValue(DeviceStatusProperty);
        set => SetValue(DeviceStatusProperty, value);
    }

    public static readonly BindableProperty NameFontStyleProperty = BindableProperty.Create(
        propertyName: nameof(NameFontStyle),
        returnType: typeof(Style),
        declaringType: typeof(DeviceTemplate));

    public Style NameFontStyle
    {
        get => (Style)GetValue(NameFontStyleProperty);
        set => SetValue(NameFontStyleProperty, value);
    }

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(BindingContext) && BindingContext is DeviceBindableModel model)
        {
            model.PropertyChanged += OnBindingContextPropertyChanged;
        }
    }

    #endregion

    #region -- Private helpers --

    private void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DeviceBindableModel.Status) && sender is DeviceBindableModel model)
        {
            var resourceStyles = App.Current.Resources.MergedDictionaries.Last();

            var styleKey = model.Status switch
            {
                EDeviceStatus.On => "tstyle_i12",
                EDeviceStatus.Off => "tstyle_i10",
                EDeviceStatus.Disconnected => "tstyle_i14",
                _ => "tstyle_i10",
            };

            statusLabel.Style = resourceStyles[styleKey] as Style;
        }
    }

    #endregion
}
