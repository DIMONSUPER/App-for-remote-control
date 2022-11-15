using System.Windows.Input;

namespace SmartMirror.Resources.DataTemplates;

public partial class SimpleNotificationsHeaderTemplate : Border
{
	public SimpleNotificationsHeaderTemplate()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
       propertyName: nameof(Title),
       returnType: typeof(string),
       declaringType: typeof(SimpleNotificationsHeaderTemplate));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
       propertyName: nameof(ImageSource),
       returnType: typeof(string),
       declaringType: typeof(SimpleNotificationsHeaderTemplate));

    public string ImageSource
    {
        get => (string)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
       propertyName: nameof(IsToggled),
       returnType: typeof(bool),
       declaringType: typeof(SimpleNotificationsHeaderTemplate),
       defaultBindingMode: BindingMode.TwoWay);

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    #endregion
}
