using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Tabs;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using SmartMirror.Controls;
using Button = Android.Widget.Button;

namespace SmartMirror.Platforms.Android.Renderers;

public class CustomTabbedPageRenderer : TabbedPageRenderer
{
    private global::Android.Views.View _tabBarView;
    private Page _previousPage;
    private bool _isDisposed;

    public CustomTabbedPageRenderer(Context context) : base(context)
    {
    }

    #region -- Protected properties --

    private TabLayout _tabBar;
    protected TabLayout TabBar => _tabBar ??= SearchInChildren<TabLayout>();

    private ViewPager _pager;
    protected ViewPager Pager => _pager ??= SearchInChildren<ViewPager>();

    protected IPageController PageController => Element as IPageController;

    protected CustomTabbedPage CustomTabbedPage => Element as CustomTabbedPage;

    #endregion

    #region -- Overrides --

    protected override void Dispose(bool disposing)
    {
        _isDisposed = true;

        base.Dispose(disposing);
    }

    protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
    {
        base.OnElementChanged(e);

        TabBar.Visibility = ViewStates.Gone;

        if (_tabBarView == null)
        {
            _tabBarView = CreateTabBarView();

            AddView(_tabBarView);
        }
    }

    protected override void OnLayout(bool changed, int l, int t, int r, int b)
    {
        //base.OnLayout(changed, l, t, r, b);
        var tabsHeight = (int)CustomTabbedPage.TabBarHeight;
        var nativeTabsHeight = (int)(tabsHeight * Context.Resources.DisplayMetrics.Density);

        var width = r - l;
        var height = b - t;

        Pager.Measure(MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.AtMost), MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.AtMost));

        if (width > 0 && height > 0)
        {
            PageController.ContainerArea = new(0, tabsHeight, Context.FromPixels(width), Context.FromPixels(height) - tabsHeight);

            SetNavigationRendererPadding(tabsHeight, 0);

            Pager.Layout(0, nativeTabsHeight, width, b);

            _tabBarView.Measure(MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.AtMost), MeasureSpec.MakeMeasureSpec(nativeTabsHeight, MeasureSpecMode.AtMost));

            _tabBarView.Layout(0, 0, width, nativeTabsHeight);
        }

        UpdateLayout(((IElementController)Element).LogicalChildren);
    }

    #endregion

    #region -- Private helpers --

    private void UpdateLayout(IEnumerable<Element> children)
    {
        foreach (var element in children)
        {
            if (element is VisualElement visualElement)
            {
                var renderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.Platform.GetRenderer(visualElement);

                if (renderer == null && CompressedLayout.GetIsHeadless(visualElement))
                {
                    UpdateLayout(((IElementController)visualElement).LogicalChildren);
                }

                renderer?.UpdateLayout();
            }
        }
    }

    private global::Android.Views.View CreateTabBarView()
    {
        var renderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.Platform.GetRenderer(CustomTabbedPage.TabBarView);

        if (renderer is null)
        {
            renderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.Platform.CreateRendererWithContext(CustomTabbedPage.TabBarView, Context);
            Microsoft.Maui.Controls.Compatibility.Platform.Android.Platform.SetRenderer(CustomTabbedPage.TabBarView, renderer);
        }

        return renderer?.View;
    }

    private T SearchInChildren<T>() where T : class
    {
        T res = null;

        for (int i = 0; i < ChildCount; i++)
        {
            if (GetChildAt(i) is T child)
            {
                res = child;
            }
        }

        return res;
    }

    private void SetNavigationRendererPadding(int paddingTop, int paddingBottom)
    {
        for (var i = 0; i < PageController.InternalChildren.Count; i++)
        {
            if (PageController.InternalChildren[i] is VisualElement child
                && Microsoft.Maui.Controls.Compatibility.Platform.Android.Platform.GetRenderer(child) is NavigationPageRenderer navigationRenderer)
            {
                navigationRenderer.SetPadding(navigationRenderer.PaddingLeft, paddingTop, navigationRenderer.PaddingRight, PaddingBottom);
            }
        }
    }

    #endregion
}

