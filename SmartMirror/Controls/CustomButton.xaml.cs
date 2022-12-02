using SmartMirror.Enums;
using System.Windows.Input;

namespace SmartMirror.Controls;

public partial class CustomButton : Border
{
    private bool _isHintShown;

	public CustomButton()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty ModeProperty = BindableProperty.Create(
        propertyName: nameof(Mode),
        returnType: typeof(EButtonMode),
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public EButtonMode Mode
    {
        get => (EButtonMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
        propertyName: nameof(IsToggled),
        returnType: typeof(bool),
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public static readonly BindableProperty HintDelayMilisecondsProperty = BindableProperty.Create(
        propertyName: nameof(HintDelayMilliseconds),
        returnType: typeof(float),
        defaultValue: 1500f,
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public float HintDelayMilliseconds
    {
        get => (float)GetValue(HintDelayMilisecondsProperty);
        set => SetValue(HintDelayMilisecondsProperty, value);
    }

    public static readonly BindableProperty IconSizesProperty = BindableProperty.Create(
        propertyName: nameof(IconSizes),
        returnType: typeof(Size),
        defaultValue: new Size(18, 18),
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public Size IconSizes
    {
        get => (Size)GetValue(IconSizesProperty);
        set => SetValue(IconSizesProperty, value);
    }

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        propertyName: nameof(IconColor),
        returnType: typeof(Color),
        defaultValue: Colors.Black,
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public static readonly BindableProperty ToggledIconColorProperty = BindableProperty.Create(
        propertyName: nameof(ToggledIconColor),
        returnType: typeof(Color),
        defaultValue: Colors.Blue,
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public Color ToggledIconColor
    {
        get => (Color)GetValue(ToggledIconColorProperty);
        set => SetValue(ToggledIconColorProperty, value);
    }

    public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
        propertyName: nameof(IconSource),
        returnType: typeof(string),
        defaultValue: string.Empty,
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public string IconSource
    {
        get => (string)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public static readonly BindableProperty ToggledIconSourceProperty = BindableProperty.Create(
        propertyName: nameof(ToggledIconSource),
        returnType: typeof(string),
        defaultValue: string.Empty,
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public string ToggledIconSource
    {
        get => (string)GetValue(ToggledIconSourceProperty);
        set => SetValue(ToggledIconSourceProperty, value);
    }

    public static readonly BindableProperty TextStyleProperty = BindableProperty.Create(
        propertyName: nameof(TextStyle),
        returnType: typeof(Style),
        declaringType: typeof(CustomButton),
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
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty ToggledTextProperty = BindableProperty.Create(
        propertyName: nameof(ToggledText),
        returnType: typeof(string),
        defaultValue: string.Empty,
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public string ToggledText
    {
        get => (string)GetValue(ToggledTextProperty);
        set => SetValue(ToggledTextProperty, value);
    }

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        propertyName: nameof(Command),
        returnType: typeof(ICommand),
        declaringType: typeof(CustomButton),
        defaultBindingMode: BindingMode.OneWay);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    #endregion

    #region -- Private helpers --

    private void OnTapped(object sender, TappedEventArgs e)
    {
        if (!_isHintShown)
        {
            switch (Mode)
            {
                case EButtonMode.Toggle:
                    IsToggled = !IsToggled;
                    break;

                case EButtonMode.Hint:
                    IsToggled = true;

                    Dispatcher.StartTimer(TimeSpan.FromMilliseconds(HintDelayMilliseconds), () => IsToggled = _isHintShown = false);
                    break;
            }

            Command?.Execute(null); 
        }
    } 

    #endregion
}