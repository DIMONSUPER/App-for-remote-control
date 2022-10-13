namespace SmartMirror.ViewModels
{
    public class BaseViewModel : BindableBase, IPageLifecycleAware, INavigationAware, IInitialize, IApplicationLifecycleAware
    {
        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        #region -- Protected properties --

        protected INavigationService NavigationService { get; }

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

        #region -- Public helpers --

        public virtual bool OnBackButtonPressed()
        {
            NavigationService.GoBackAsync();

            return true;
        }

        #endregion
    }
}

