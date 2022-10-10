using SmartMirror.Enums;

namespace SmartMirror.ViewModels.Tabs;

public class BaseTabViewModel : BindableBase, IInitialize, IInitializeAsync, IPageLifecycleAware
{
    public BaseTabViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;

        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    ~BaseTabViewModel() => Connectivity.ConnectivityChanged -= OnConnectivityChanged;

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

    public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

    #endregion

    #region -- IInitialize implementation --

    public virtual void Initialize(INavigationParameters parameters)
    {
    }

    #endregion

    #region -- IInitializeAsync implementation --

    public virtual Task InitializeAsync(INavigationParameters parameters)
    {
        return Task.CompletedTask;
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

    #region -- Private helpers --

    private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        ConnectivityChanged?.Invoke(sender, e);
    }

    #endregion
}

