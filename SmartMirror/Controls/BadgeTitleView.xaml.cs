namespace SmartMirror.Controls;

public partial class BadgeTitleView : HorizontalStackLayout
{
	public BadgeTitleView()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        propertyName: nameof(Title),
        returnType: typeof(string),
        declaringType: typeof(BadgeTitleView),
        defaultBindingMode: BindingMode.OneWay);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty NumberProperty = BindableProperty.Create(
        propertyName: nameof(Number),
        returnType: typeof(int),
        declaringType: typeof(BadgeTitleView),
        defaultBindingMode: BindingMode.OneWay);

    public int Number
    {
        get => (int)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    #endregion
}