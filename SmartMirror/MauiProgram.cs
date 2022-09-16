using System.Diagnostics;
using Prism.Navigation;
using SmartMirror.ViewModels;
using SmartMirror.Views;

namespace SmartMirror;

public static class MauiProgram
{
    #region -- Public static helpers --

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UsePrism(prism => prism.RegisterTypes(RegisterTypes).OnAppStart(OnAppStart))
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Medium-500.ttf", "InterMedium");
                fonts.AddFont("Inter-SemiBold-600.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold-700.ttf", "InterBold");
            });

        return builder.Build();
    }

    #endregion

    #region -- Private static helpers --

    private static void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<MainPage>();
    }

    private static void OnAppStart(INavigationService navigationService)
    {
        navigationService.CreateBuilder().AddNavigationPage().AddSegment<MainPageViewModel>().Navigate(HandleErrors);
    }

    private static void HandleErrors(Exception exception)
    {
        Debug.WriteLine(exception.Message);
        Debugger.Break();
    }

    #endregion
}

