using System;
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
                .AddSegment<MainPageViewModel>()
                .Navigate();
        }

        #endregion
    }
}

