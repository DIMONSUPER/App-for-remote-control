using System.Windows.Input;

namespace SmartMirror.Controls;

public partial class ActivityIndicatorButton : Frame
{
    #region -- Public property --

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        propertyName: nameof(Command),
        returnType: typeof(ICommand),
        declaringType: typeof(ActivityIndicatorButton),
        propertyChanged: OnCommandPropertyChanged);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        propertyName: nameof(Text),
        returnType: typeof(string),
        declaringType: typeof(ActivityIndicatorButton));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        propertyName: nameof(TextColor),
        returnType: typeof(Color),
        declaringType: typeof(ActivityIndicatorButton));

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        propertyName: nameof(FontSize),
        returnType: typeof(int),
        declaringType: typeof(ActivityIndicatorButton));

    public int FontSize
    {
        get => (int)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    //public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
    //    propertyName: nameof(BorderColor),
    //    returnType: typeof(Color),
    //    declaringType: typeof(ActivityIndicatorButton));

    //public Color BorderColor
    //{
    //    get => (Color)GetValue(BorderColorProperty);
    //    set => SetValue(BorderColorProperty, value);
    //}

    public static readonly BindableProperty BorderHeightProperty = BindableProperty.Create(
        propertyName: nameof(BorderHeight),
        returnType: typeof(int),
        declaringType: typeof(ActivityIndicatorButton));

    public int BorderHeight
    {
        get => (int)GetValue(BorderHeightProperty);
        set => SetValue(BorderHeightProperty, value);
    }

    public static readonly BindableProperty ActivityIndicatorColorProperty = BindableProperty.Create(
        propertyName: nameof(ActivityIndicatorColor),
        returnType: typeof(Color),
        declaringType: typeof(ActivityIndicatorButton));

    public Color ActivityIndicatorColor
    {
        get => (Color)GetValue(ActivityIndicatorColorProperty);
        set => SetValue(ActivityIndicatorColorProperty, value);
    }

    #endregion

    #region -- Public helpers --

    public void SubscribeForCanExecuteChange(ICommand command)
    {
        command.CanExecuteChanged += OnCommandCanExecuteChanged;
    }

    public void UnsubscribeFromCanExecuteChange(ICommand command)
    {
        command.CanExecuteChanged -= OnCommandCanExecuteChanged;
    }

    #endregion

    #region -- Private helpers --

    private static void OnCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ActivityIndicatorButton button && newValue is ICommand newCommand)
        {
            if (oldValue is ICommand oldCommand)
            {
                button.UnsubscribeFromCanExecuteChange(oldCommand);
            }

            button.SubscribeForCanExecuteChange(newCommand);
        }
    }

    private void OnCommandCanExecuteChanged(object sender, EventArgs e)
    {
        IsEnabled = Command.CanExecute(null);
    }

    #endregion
}
