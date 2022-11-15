namespace SmartMirror.Resources.DataTemplates;

public partial class SimpleAccessoryTemplate : Border
{
	public SimpleAccessoryTemplate()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty HasSwitchProperty = BindableProperty.Create(
       propertyName: nameof(HasSwitch),
       returnType: typeof(bool),
       declaringType: typeof(SimpleAccessoryTemplate));

    public bool HasSwitch
    {
        get => (bool)GetValue(HasSwitchProperty);
        set => SetValue(HasSwitchProperty, value);
    }

    #endregion
}