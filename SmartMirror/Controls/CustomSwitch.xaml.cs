﻿using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartMirror.Controls;

public partial class CustomSwitch : ContentView
{
    public CustomSwitch()
    {
        InitializeComponent();

        UpdateSwitchLayout();
    }

    #region -- Public properties --

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
        propertyName: nameof(IsToggled),
        returnType: typeof(bool),
        declaringType: typeof(CustomSwitch),
        defaultBindingMode: BindingMode.TwoWay);

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

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        propertyName: nameof(CornerRadius),
        returnType: typeof(double),
        declaringType: typeof(CustomSwitch));

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty ThumbSizeProperty = BindableProperty.Create(
        propertyName: nameof(ThumbSize),
        returnType: typeof(double),
        declaringType: typeof(CustomSwitch));

    public double ThumbSize
    {
        get => (double)GetValue(ThumbSizeProperty);
        set => SetValue(ThumbSizeProperty, value);
    }

    public static readonly BindableProperty DisabledCommandProperty = BindableProperty.Create(
        propertyName: nameof(DisabledCommand),
        returnType: typeof(ICommand),
        declaringType: typeof(CustomSwitch));

    public ICommand DisabledCommand
    {
        get => (ICommand)GetValue(DisabledCommandProperty);
        set => SetValue(DisabledCommandProperty, value);
    }

    public static readonly BindableProperty DisabledCommandParameterProperty = BindableProperty.Create(
        propertyName: nameof(DisabledCommandParameter),
        returnType: typeof(object),
        declaringType: typeof(CustomSwitch));

    public object DisabledCommandParameter
    {
        get => (object)GetValue(DisabledCommandParameterProperty);
        set => SetValue(DisabledCommandParameterProperty, value);
    }

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is nameof(IsToggled))
        {
            UpdateSwitchLayout();
        }
    }

    #endregion

    #region -- Private helpers --

    private void UpdateSwitchLayout()
    {
        thumb.HorizontalOptions = IsToggled ? LayoutOptions.End : LayoutOptions.Start;
        thumb.Fill = IsToggled ? OnThumbColor : OffThumbColor;
        frame.BackgroundColor = IsToggled ? OnColor : OffColor;
    }

    private void OnSwitchToggled(System.Object sender, System.EventArgs e)
    {
        if (IsEnabled)
        {
            IsToggled = !IsToggled;
        }
        else
        {
            if (DisabledCommand is not null && DisabledCommand.CanExecute(DisabledCommandParameter))
            {
                DisabledCommand?.Execute(DisabledCommandParameter);
            }
        }
    }

    #endregion
}
