using SmartMirror.Enums;

namespace SmartMirror.ViewModels.Tabs;

public class BaseTabViewModel : BindableBase, IInitialize, IPageLifecycleAware, INavigationAware, IDestructible
{
    public BaseTabViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;

        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    #region -- Protected properties --

    protected INavigationService NavigationService { get; }

    protected bool IsInternetConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    #endregion

    #region -- Public properties --

    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    private EPageState _dataState;
    public EPageState DataState
    {
        get => _dataState;
        set => SetProperty(ref _dataState, value);
    }

    #endregion

    #region -- IInitialize implementation --

    public virtual void Initialize(INavigationParameters parameters)
    {
    }

    #endregion

    #region -- IPageLifecycleAware implementation --

    public virtual void OnAppearing()
    {
    }

    public virtual void OnDisappearing()
    {
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

    #region -- Protected helpers --

    protected virtual void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
    }

    #endregion
}

