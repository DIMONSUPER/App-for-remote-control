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
		defaultBindingMode: BindingMode.OneWay,
		propertyChanged: OnTabNamesPropertyChanged);

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
		defaultBindingMode: BindingMode.TwoWay,
		propertyChanged: OnSelectedTabNamePropertyChanged);

	public string SelectedTabName
	{
		get => (string)GetValue(SelectedTabNameProperty);
		set => SetValue(SelectedTabNameProperty, value);
	}

	public static readonly BindableProperty SelectedTabIndexProperty = BindableProperty.Create(
		propertyName: nameof(SelectedTabIndex),
		returnType: typeof(int),
		defaultValue: -1,
		declaringType: typeof(TabsSelector),
		defaultBindingMode: BindingMode.OneWayToSource);

	public int SelectedTabIndex
	{
		get => (int)GetValue(SelectedTabIndexProperty);
		set => SetValue(SelectedTabIndexProperty, value);
	}

	public static readonly BindableProperty TabWidthProperty = BindableProperty.Create(
		propertyName: nameof(TabWidth),
		returnType: typeof(double),
		defaultValue: 191d,
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

    #region -- Private helpers --

    private static void OnTabNamesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TabsSelector tabsSelector && tabsSelector.TabNames is not null && tabsSelector.TabNames.Any()) 
		{
			var tabIndex = tabsSelector.SelectedTabIndex;

            tabsSelector.SelectedTabName = tabIndex >= 0 && tabIndex < tabsSelector.TabNames.Count
				? tabsSelector.TabNames.ElementAt(tabIndex)
				: tabsSelector.SelectedTabName = tabsSelector.TabNames.FirstOrDefault();	
        }
    }

    private static void OnSelectedTabNamePropertyChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is TabsSelector tabsSelector && tabsSelector.TabNames is not null && tabsSelector.TabNames.Any())
		{
			tabsSelector.SelectedTabIndex = tabsSelector.TabNames.IndexOf(tabsSelector.SelectedTabName);

			tabsSelector.SelectedTabChangedCommand?.Execute(null);
        }
	} 

	#endregion
}