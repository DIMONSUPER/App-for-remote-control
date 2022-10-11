using SmartMirror.Views.Tabs;
using System.Windows.Input;

namespace SmartMirror.Controls;

public partial class CustomStateView : ContentView
{
	public CustomStateView()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty StateProperty = BindableProperty.Create(
            propertyName: nameof(State),
            returnType: typeof(object),
            declaringType: typeof(CustomStateView));

    public object State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly BindableProperty EmptyTitleProperty = BindableProperty.Create(
        propertyName: nameof(EmptyTitle),
        returnType: typeof(string),
        declaringType: typeof(CustomStateView));

    public string EmptyTitle
    {
        get => (string)GetValue(EmptyTitleProperty);
        set => SetValue(EmptyTitleProperty, value);
    }

    public static readonly BindableProperty LoadingTitleProperty = BindableProperty.Create(
        propertyName: nameof(LoadingTitle),
        returnType: typeof(string),
        declaringType: typeof(CustomStateView));

    public string LoadingTitle
    {
        get => (string)GetValue(LoadingTitleProperty);
        set => SetValue(LoadingTitleProperty, value);
    }

    public static readonly BindableProperty CompleteContentProperty = BindableProperty.Create(
        propertyName: nameof(CompleteContent),
        returnType: typeof(View),
        declaringType: typeof(CustomStateView));

    public View CompleteContent
    {
        get => (View)GetValue(CompleteContentProperty);
        set => SetValue(CompleteContentProperty, value);
    }

    public static readonly BindableProperty EmptyStateImageProperty = BindableProperty.Create(
        propertyName: nameof(EmptyStateImage),
        returnType: typeof(string),
        declaringType: typeof(CustomStateView));

    public string EmptyStateImage
    {
        get => (string)GetValue(EmptyStateImageProperty);
        set => SetValue(EmptyStateImageProperty, value);
    }

    public static readonly BindableProperty TryAgainCommandProperty = BindableProperty.Create(
        propertyName: nameof(TryAgainCommand),
        returnType: typeof(ICommand),
        declaringType: typeof(CustomStateView));

    public ICommand TryAgainCommand
    {
        get => (ICommand)GetValue(TryAgainCommandProperty);
        set => SetValue(TryAgainCommandProperty, value);
    }

    #endregion
}
