namespace SmartMirror.Controls;

public partial class CustomImageBotton : Border
{
	public CustomImageBotton()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            propertyName: nameof(Source),
            returnType: typeof(string),
            declaringType: typeof(CustomImageBotton));

    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly BindableProperty HeightImageProperty = BindableProperty.Create(
            propertyName: nameof(HeightImage),
            returnType: typeof(double),
            declaringType: typeof(CustomImageBotton));

    public double HeightImage
    {
        get => (double)GetValue(HeightImageProperty);
        set => SetValue(HeightImageProperty, value);
    }

    #endregion
}