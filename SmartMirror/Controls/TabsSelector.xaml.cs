using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartMirror.Controls;

public partial class TabsSelector : ContentView
{
    public TabsSelector()
    {
        InitializeComponent();
    }

    #region -- Public properties --

    public static readonly BindableProperty TabNamesProperty = BindableProperty.Create(
        propertyName: nameof(TabNames),
        returnType: typeof(IList<string>),
        defaultValue: Enumerable.Empty<string>(),
        declaringType: typeof(TabsSelector),
        defaultBindingMode: BindingMode.OneWay);

    public IList<string> TabNames
    {
        get => (IList<string>)GetValue(TabNamesProperty);
        set => SetValue(TabNamesProperty, value);
    }

    public static readonly BindableProperty SelectedTabNameProperty = BindableProperty.Create(
        propertyName: nameof(SelectedTabName),
        returnType: typeof(string),
        defaultValue: string.Empty,
        declaringType: typeof(TabsSelector),
        defaultBindingMode: BindingMode.TwoWay);

    public string SelectedTabName
    {
        get => (string)GetValue(SelectedTabNameProperty);
        set => SetValue(SelectedTabNameProperty, value);
    }

    public static readonly BindableProperty TabWidthProperty = BindableProperty.Create(
        propertyName: nameof(TabWidth),
        returnType: typeof(double),
        defaultValue: 181d,
        declaringType: typeof(TabsSelector),
        defaultBindingMode: BindingMode.OneWay);

    public double TabWidth
    {
        get => (double)GetValue(TabWidthProperty);
        set => SetValue(TabWidthProperty, value);
    }

    public static readonly BindableProperty SelectedTabChangedCommandProperty = BindableProperty.Create(
        propertyName: nameof(SelectedTabChangedCommand),
        returnType: typeof(ICommand),
        defaultValue: null,
        declaringType: typeof(TabsSelector),
        defaultBindingMode: BindingMode.OneWay);

    public ICommand SelectedTabChangedCommand
    {
        get => (ICommand)GetValue(SelectedTabChangedCommandProperty);
        set => SetValue(SelectedTabChangedCommandProperty, value);
    }

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is nameof(SelectedTabName))
        {
            SelectedTabChangedCommand?.Execute(null);
        }
    }

    #endregion
}