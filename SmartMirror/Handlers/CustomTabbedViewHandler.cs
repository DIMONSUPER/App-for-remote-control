
using System.ComponentModel;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Java.Lang;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using SmartMirror.Helpers.Events;

namespace SmartMirror.Handlers;

public class CustomTabbedViewHandler : TabbedViewHandler
{
    #region -- Protected properties --

    protected Controls.CustomTabbedPage CustomTabbedPage => this.VirtualView as Controls.CustomTabbedPage;

    private Android.Views.View _tabBar;
    protected Android.Views.View TabBar => _tabBar ??= CreateTabBarView();

    private AndroidX.ViewPager2.Widget.ViewPager2 _viewPager2;
    protected AndroidX.ViewPager2.Widget.ViewPager2 ViewPager2 => _viewPager2 ??= PlatformView as AndroidX.ViewPager2.Widget.ViewPager2;

    private FragmentManager _fragmentManager;
    protected FragmentManager FragmentManager => _fragmentManager ??= (Platform.CurrentActivity as MauiAppCompatActivity)?.SupportFragmentManager;

    private NavigationRootManager _rootManager;
    protected NavigationRootManager RootManager => _rootManager ??= MauiContext?.Services?.GetRequiredService<NavigationRootManager>();

    private IEventAggregator _eventAggregator;
    protected IEventAggregator EventAggregator => _eventAggregator ??= MauiContext?.Services?.GetRequiredService<IEventAggregator>();

    #endregion

    #region -- Overrides --

    protected override void DisconnectHandler(Android.Views.View platformView)
    {
        base.DisconnectHandler(platformView);

        FragmentManager.FragmentOnAttach -= OnAttached;

        EventAggregator.GetEvent<HideTabsTabbedViewEvent>().Unsubscribe(OnHideTabsEvent);
    }

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);

        FragmentManager.FragmentOnAttach += OnAttached;

        EventAggregator.GetEvent<HideTabsTabbedViewEvent>().Subscribe(OnHideTabsEvent);
    }

    #endregion

    #region -- Private helpers --

    private global::Android.Views.View CreateTabBarView()
    {
        ViewPager2.OffscreenPageLimit = 5;

        var handler = this.CustomTabbedPage.TabBarView.Handler;

        if (handler is null)
        {
            handler = this.CustomTabbedPage.TabBarView.ToHandler(this.MauiContext);
        }

        return handler.PlatformView as global::Android.Views.View;
    }

    private void AddTabBar()
    {
        var last = CustomTabbedPage.Navigation.NavigationStack[^1];

        if (RootManager.RootView is ViewGroup viewGroup && TabBar.Parent is null)
        {
            viewGroup.AddView(TabBar, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

            ViewPager2?.SetPadding(0, (int)Context.ToPixels(CustomTabbedPage.TabBarHeight), 0, 0);
        }
    }

    private void OnAttached(object sender, FragmentOnAttachEventArgs e)
    {
        var fragmentManager = e.P0;
        var fragment = e.P1;

        if (fragment.Id == Resource.Id.navigationlayout_toptabs)
        {
            fragmentManager
                .BeginTransaction()
                .Remove(fragment)
                .RunOnCommit(new Runnable(AddTabBar))
                .SetReorderingAllowed(true)
                .Commit();
        }
    }

    private void OnHideTabsEvent(bool state)
    {
        //Hack: If modal navigation - we don't need to remove tab bar
        if (!CustomTabbedPage.Navigation.ModalStack.Any() && TabBar.Parent is not null)
        {
            FragmentManager.BeginTransaction().RunOnCommit(new Runnable(() => TabBar?.Post(() => TabBar?.RemoveFromParent()))).Commit();
        }
    }

    #endregion

    private class Runnable : Java.Lang.Object, IRunnable
    {
        private readonly Action _action;

        public Runnable(Action action)
        {
            _action = action;
        }

        #region -- IRunnable implementaion --

        public void Run()
        {
            _action?.Invoke();
        }

        #endregion
    }

    private class CustomFragment : Fragment
    {
        private readonly Android.Views.View _view;

        public CustomFragment(Android.Views.View view)
        {
            _view = view;
        }

        #region -- Overrides --

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return _view;
        }

        #endregion
    }
}

