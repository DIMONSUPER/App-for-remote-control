using System.Diagnostics;

namespace SmartMirror.ViewModels
{
    public class SplashScreenPageViewModel : INavigationAware
    {
        private readonly INavigationService _navigationService;

        public SplashScreenPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        #region -- INavigationAware implementaion --

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            await Task.Delay(2000);

            _navigationService.CreateBuilder()
                .AddSegment<MainTabbedPageViewModel>()
                .Navigate(HandleErrors);
        }

        #endregion

        #region -- Private helpers --

        private static void HandleErrors(Exception exception)
        {
            Debug.WriteLine(exception.Message);
            Debugger.Break();
        }

        #endregion
    }
}

