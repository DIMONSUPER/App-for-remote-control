using System;
using System.Diagnostics;
using Prism.Navigation;

namespace SmartMirror.ViewModels
{
    public class SplashScreenPageViewModel : BaseViewModel
    {
        public SplashScreenPageViewModel(
            INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Overrides --

        public async override void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(2000);

            NavigationService.CreateBuilder()
                .AddNavigationPage()
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

