using System.Windows.Input;

namespace SmartMirror.Controls;

public partial class ButtonLoader : Grid
{
	public ButtonLoader()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty ButtonTextProperty = BindableProperty.Create(
        propertyName: nameof(ButtonText),
        returnType: typeof(string),
        declaringType: typeof(ButtonLoader));

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public static readonly BindableProperty ButtonStyleProperty = BindableProperty.Create(
        propertyName: nameof(ButtonStyle),
        returnType: typeof(Style),
        declaringType: typeof(ButtonLoader));

    public Style ButtonStyle
    {
        get => (Style)GetValue(ButtonStyleProperty);
        set => SetValue(ButtonStyleProperty, value);
    }

    public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(
        propertyName: nameof(IsRunning),
        returnType: typeof(bool),
        declaringType: typeof(ButtonLoader));

    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        propertyName: nameof(Command),
        returnType: typeof(ICommand),
        declaringType: typeof(ButtonLoader));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        propertyName: nameof(CommandParameter),
        returnType: typeof(object),
        declaringType: typeof(ButtonLoader));

    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    #endregion
}
