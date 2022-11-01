using Microsoft.Maui.Controls.Shapes;
using SmartMirror.Interfaces;

namespace SmartMirror.Controls;

public class CustomTabbedPage : TabbedPage
{
    private List<View> _tabCells = new();
    private List<View> _selectedTabCells = new();
    private HorizontalStackLayout _tabsStackLayout;

    public CustomTabbedPage()
    {
    }

    #region -- Public properties --

    public static readonly BindableProperty TabBarHeightProperty = BindableProperty.Create(
        propertyName: nameof(TabBarHeight),
        returnType: typeof(double),
        declaringType: typeof(CustomTabbedPage),
        defaultValue: 123d);

    public double TabBarHeight
    {
        get => (double)GetValue(TabBarHeightProperty);
        set => SetValue(TabBarHeightProperty, value);
    }

    public static readonly BindableProperty TabBarCellTemplateProperty = BindableProperty.Create(
        propertyName: nameof(TabBarCellTemplate),
        returnType: typeof(DataTemplate),
        declaringType: typeof(CustomTabbedPage));

    public DataTemplate TabBarCellTemplate
    {
        get => (DataTemplate)GetValue(TabBarCellTemplateProperty);
        set => SetValue(TabBarCellTemplateProperty, value);
    }

    public static readonly BindableProperty SelectedTabBarCellTemplateProperty = BindableProperty.Create(
        propertyName: nameof(SelectedTabBarCellTemplate),
        returnType: typeof(DataTemplate),
        declaringType: typeof(CustomTabbedPage));

    public DataTemplate SelectedTabBarCellTemplate
    {
        get => (DataTemplate)GetValue(SelectedTabBarCellTemplateProperty);
        set => SetValue(SelectedTabBarCellTemplateProperty, value);
    }

    public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
        propertyName: nameof(BorderWidth),
        returnType: typeof(double),
        declaringType: typeof(CustomTabbedPage),
        defaultValue: 1d);

    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    public static readonly BindableProperty BorderRadiusProperty = BindableProperty.Create(
        propertyName: nameof(BorderRadius),
        returnType: typeof(double),
        declaringType: typeof(CustomTabbedPage),
        defaultValue: 40d);

    public double BorderRadius
    {
        get => (double)GetValue(BorderRadiusProperty);
        set => SetValue(BorderRadiusProperty, value);
    }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        propertyName: nameof(BorderColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomTabbedPage),
        defaultValue: Color.FromArgb("#FFFFFF"));

    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    private Grid _tabBarView;
    public Grid TabBarView => _tabBarView ??= CreateTabBar();

    #endregion

    #region -- Overrides --

    protected override void OnCurrentPageChanged()
    {
        if (_tabsStackLayout is null)
        {
            return;
        }

        var currentIndex = Children.IndexOf(CurrentPage);

        for (int i = 0; i < Children.Count; i++)
        {
            _tabsStackLayout[i] = i == currentIndex
                ? _selectedTabCells[i]
                : _tabCells[i];

            if (Children[i] is NavigationPage navigationPage && navigationPage.CurrentPage.BindingContext is ISelectable viewModel)
            {
                viewModel.IsSelected = i == currentIndex;
            }
        }
    }

    #endregion

    #region -- Private helpers --

    private Grid CreateTabBar()
    {
        var tabBarView = new Grid
        {
            HeightRequest = TabBarHeight,
            RowSpacing = 0,
        };

        var stackTimeAndTabs = new StackLayout() { Spacing = 0 };

        stackTimeAndTabs.Add(CreateTimeLabel());

        stackTimeAndTabs.Add(CreateBorder());

        tabBarView.Add(stackTimeAndTabs);

        OnCurrentPageChanged();

        return tabBarView;
    }

    private CurrentTimeControl CreateTimeLabel()
    {
        var currentTime = new CurrentTimeControl
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new(60, 9, 0, 9),
            TextColor = Color.FromArgb("#FFFFFF"),
            FontFamily = "InterSemiBold",
        };

        return currentTime;
    }

    private Border CreateBorder()
    {
        var border = new Border
        {
            HorizontalOptions = LayoutOptions.Center,
            StrokeThickness = BorderWidth,
            StrokeShape = new RoundRectangle() { CornerRadius = BorderRadius },
            Stroke = BorderColor,
            BackgroundColor = Color.FromArgb("#801F1F1F"),
        };

        border.Content = CreateTabsStackLayout();

        return border;
    }

    private HorizontalStackLayout CreateTabsStackLayout()
    {
        _selectedTabCells.Clear();
        _tabCells.Clear();

        var tabsStackLayout = new HorizontalStackLayout
        {
            Spacing = 0,
            Margin = 0,
            Padding = new(30, 0),
        };

        var tabTappedGestureRecognizer = new TapGestureRecognizer();
        tabTappedGestureRecognizer.Tapped += OnTabTapped;

        foreach (var page in Children)
        {
            var tab = TabBarCellTemplate.CreateContent() as View;
            tab.GestureRecognizers.Add(tabTappedGestureRecognizer);

            var selectedTab = SelectedTabBarCellTemplate.CreateContent() as View;

            selectedTab.BindingContext = tab.BindingContext = page;

            _selectedTabCells.Add(selectedTab);
            _tabCells.Add(tab);

            tabsStackLayout.Add(tab);
        }

        _tabsStackLayout = tabsStackLayout;

        return tabsStackLayout;
    }

    private void OnTabTapped(object sender, EventArgs e)
    {
        if (sender is IView view)
        {
            var currentIndex = _tabsStackLayout.IndexOf(view);

            MainThread.BeginInvokeOnMainThread(() => CurrentPage = Children[currentIndex]);
        }
    }

    #endregion
}

