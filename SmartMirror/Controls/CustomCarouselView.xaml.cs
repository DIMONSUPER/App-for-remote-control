using System.Collections;
using System.Runtime.CompilerServices;
using SmartMirror.ViewModels.Tabs;

namespace SmartMirror.Controls;

public partial class CustomCarouselView : ContentView
{
    public CustomCarouselView()
    {
        InitializeComponent();
    }

    #region -- Public properties --

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(ItemsSource),
        returnType: typeof(IList<BaseTabViewModel>),
        declaringType: typeof(CustomCarouselView),
        defaultValue: default);

    public IList<BaseTabViewModel> ItemsSource
    {
        get => (IList<BaseTabViewModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
        propertyName: nameof(SelectedColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomCarouselView),
        defaultValue: Color.FromArgb("#E9E9E9"));

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        propertyName: nameof(TextColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomCarouselView));

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(
        propertyName: nameof(SelectedTextColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomCarouselView),
        defaultValue: Color.FromArgb("#000000"));

    public Color SelectedTextColor
    {
        get => (Color)GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == ItemsSourceProperty.PropertyName)
        {
            OnItemsSourceChanged();
        }
    }

    #endregion

    #region -- Private helpers --

    private void OnItemsSourceChanged()
    {
        ClearPreviouseItems();
        SetTabs();
    }

    private void SetTabs()
    {
        var views = carouselView.ItemsSource.Cast<View>().ToList();

        if (ItemsSource is not null && ItemsSource.Any())
        {
            for (int i = 0; i < ItemsSource.Count; i++)
            {
                if (i == 0)
                {
                    ItemsSource[i].IsSelected = true;
                }

                views[i].BindingContext = ItemsSource[i];
                ItemsSource[i].Initialize(new NavigationParameters());
                tabsStackLayout.Add(CreateTabView(ItemsSource[i]));
            }
        }
    }

    private void ClearPreviouseItems()
    {
        if (tabsStackLayout.Any())
        {
            foreach (var item in tabsStackLayout)
            {
                if (item is View view)
                {
                    view.GestureRecognizers.Clear();
                }
            }

            tabsStackLayout.Clear();
        }
    }

    private void OnTabViewTapped(object sender, EventArgs e)
    {
        var index = tabsStackLayout.IndexOf(sender as IView);

        if (index > -1 && ItemsSource is not null && ItemsSource.Any())
        {
            carouselView.ScrollTo(index, animate: false);

            for (int i = 0; i < ItemsSource.Count; i++)
            {
                ItemsSource[i].IsSelected = i == index;
            }
        }
    }

    private Grid CreateTabView(BaseTabViewModel viewModel)
    {
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            Padding = new(0, 12),
        };

        grid.Add(CreateBoxView(viewModel));
        grid.Add(CreateLabel(viewModel));

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnTabViewTapped;
        grid.GestureRecognizers.Add(tapGesture);

        return grid;
    }

    private BoxView CreateBoxView(BaseTabViewModel viewModel)
    {
        var boxView = new BoxView
        {
            Color = Color.FromRgba(0, 0, 0, 0),
            CornerRadius = 35,
            Shadow = null,
        };

        var dataTrigger = new DataTrigger(typeof(BoxView))
        {
            Binding = new Binding(path: nameof(viewModel.IsSelected), source: viewModel),
            Value = true,
        };

        dataTrigger.Setters.Add(new()
        {
            Property = BoxView.ColorProperty,
            Value = SelectedColor,
        });

        dataTrigger.Setters.Add(new()
        {
            Property = BoxView.ShadowProperty,
            Value = new Shadow
            {
                Brush = Brush.Black,
                Opacity = 0.5f,
                Offset = new(0, 17),
            },
        });

        boxView.Triggers.Add(dataTrigger);

        return boxView;
    }

    private Label CreateLabel(BaseTabViewModel viewModel)
    {
        var label = new Label
        {
            Text = viewModel.Title,
            Padding = new(42, 12),
        };

        label.SetBinding(Label.TextProperty, new Binding(nameof(viewModel.Title), source: viewModel));
        label.SetDynamicResource(Label.StyleProperty, "tstyle_i10");

        var dataTrigger = new DataTrigger(typeof(Label))
        {
            Binding = new Binding(path: nameof(viewModel.IsSelected), source: viewModel),
            Value = true,
        };

        dataTrigger.Setters.Add(new()
        {
            Property = Label.TextColorProperty,
            Value = SelectedTextColor,
        });

        label.Triggers.Add(dataTrigger);

        return label;
    }

    #endregion
}
