using SmartMirror.Enums;
using SmartMirror.Interfaces;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class BaseTabViewModel : BindableBase, IPageLifecycleAware, INavigationAware, IDestructible, ISelectable
{
    public BaseTabViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;

        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    #region -- Protected properties --

    protected INavigationService NavigationService { get; }

    protected bool IsInternetConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    protected bool IsDataLoading => DataState 
        is EPageState.Loading 
        or EPageState.NoInternetLoader 
        or EPageState.LoadingSkeleton;

    #endregion

    #region -- Public properties --

    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private EPageState _dataState;
    public EPageState DataState
    {
        get => _dataState;
        set => SetProperty(ref _dataState, value);
    }

    private bool _isPageFocused;
    public bool IsPageFocused
    {
        get => _isPageFocused;
        set => SetProperty(ref _isPageFocused, value);
    }

    private bool _isNeedReloadData = true;
    public bool IsNeedReloadData
    {
        get => _isNeedReloadData;
        set => SetProperty(ref _isNeedReloadData, value);
    }

    #endregion

    #region -- IPageLifecycleAware implementation --

    public virtual void OnAppearing()
    {
        IsPageFocused = true;
    }

    public virtual void OnDisappearing()
    {
        IsPageFocused = false;
    }

    #endregion

    #region -- INavigationAware implementation --

    public virtual void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public virtual void OnNavigatedTo(INavigationParameters parameters)
    {
    }

    #endregion

    #region -- IDestructible implementation --

    public virtual void Destroy()
    {
        Connectivity.ConnectivityChanged -= OnConnectivityChanged;
    }

    #endregion

    #region -- ISelectable implementaion --

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    #endregion

    #region -- Protected helpers --

    protected virtual void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
    }

    #endregion
}

