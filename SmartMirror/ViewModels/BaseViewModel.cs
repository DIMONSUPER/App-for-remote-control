using SmartMirror.Enums;

namespace SmartMirror.ViewModels
{
    public class BaseViewModel : BindableBase, IPageLifecycleAware, INavigationAware, IInitialize, IApplicationLifecycleAware
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

