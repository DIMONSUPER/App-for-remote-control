using SmartMirror.Controls;
using System.ComponentModel;

namespace SmartMirror.Views.Tabs.Pages;

public class BaseTabContentPage : ContentPage, INotifyPropertyChanged
{
    public BaseTabContentPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
    }

    #region -- Public properties  --

    public LazyView LazyView { get; set; }

    #endregion

    #region -- Overrides --

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LazyView?.LoadView();

        if (LazyView?.Content is IPageLifecycleAware content)
        {
            content.OnAppearing();
        }
    }

    #endregion
}