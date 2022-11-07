namespace SmartMirror.Controls;

public partial class TitleAndSwitchView : Grid
{
	public TitleAndSwitchView()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        propertyName: nameof(Title),
        returnType: typeof(string),
        declaringType: typeof(TitleAndSwitchView),
        defaultBindingMode: BindingMode.OneWay);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
        propertyName: nameof(IsToggled),
        returnType: typeof(bool),
        declaringType: typeof(TitleAndSwitchView),
        defaultBindingMode: BindingMode.TwoWay);

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    #endregion
}