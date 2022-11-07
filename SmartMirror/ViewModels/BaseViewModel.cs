using SmartMirror.Enums;

namespace SmartMirror.ViewModels
{
    public class BaseViewModel : BindableBase, IPageLifecycleAware, INavigationAware, IInitialize, IApplicationLifecycleAware, IDestructible
    {
        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;

            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        #region -- Protected properties --

        protected INavigationService NavigationService { get; }

        protected bool IsInternetConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        #endregion

        #region -- Public properties --

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

        #region -- IInitialize implementation --

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        #endregion

        #region -- IApplicationLifecycleAware implementation --

        public virtual void OnResume()
        {
        }

        public virtual void OnSleep()
        {
        }

        #endregion

        #region -- IDestructible implementation --

        public virtual void Destroy()
        {
        }

        #endregion

        #region -- Protected helpers --

        protected virtual void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
        }

        #endregion

        #region -- Public helpers --

        public virtual bool OnBackButtonPressed()
        {
            NavigationService.GoBackAsync();

            return true;
        }

        #endregion
    }
}

