using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using SmartMirror.Controls;
using SmartMirror.Handlers;
using SmartMirror.Platforms.Android.Renderers;
//using SmartMirror.Platforms.Services;
//using SmartMirror.Services.Amazon;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mock;
using SmartMirror.Services.Notifications;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;
using SmartMirror.Services.Scenarios;
using SmartMirror.ViewModels;
using SmartMirror.ViewModels.Dialogs;
using SmartMirror.Views;
using SmartMirror.Views.Dialogs;
using SmartMirror.Views.Tabs;
using System.Diagnostics;
using SmartMirror.Services.RoomsService;

namespace SmartMirror;

public static class MauiProgram
{
    #region -- Public static helpers --

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCompatibility()
            .ConfigureMauiHandlers(OnConfigureMauiHandlers)
            .UsePrism(prism => prism.RegisterTypes(RegisterTypes).OnAppStart(OnAppStart))
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Medium-500.ttf", "InterMedium");
                fonts.AddFont("Inter-SemiBold-600.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold-700.ttf", "InterBold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(Video), typeof(VideoHandler));
            });

        builder.Services.AddLocalization();

        return builder.Build();
    }

    #endregion

    #region -- Private static helpers --

    private static void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterDialog<ErrorDialog>();
        containerRegistry.RegisterDialog<TemporaryDialog>();

        containerRegistry.RegisterForNavigation<SplashScreenPage>();
        containerRegistry.RegisterForNavigation<MainTabbedPage>();
        containerRegistry.RegisterForNavigation<RoomsPage>();
        containerRegistry.RegisterForNavigation<NotificationsPage>();
        containerRegistry.RegisterForNavigation<CamerasPage>();
        containerRegistry.RegisterForNavigation<ScenariosPage>();
        containerRegistry.RegisterForNavigation<RoomPage>();
        containerRegistry.RegisterForNavigation<ScenarioPage>();

        containerRegistry.RegisterSingleton<IMapperService, MapperService>();
        containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
        containerRegistry.RegisterSingleton<IRestService, RestService>();
        //TODO: Remove when companion app is ready
        //containerRegistry.RegisterSingleton<IAmazonService, AmazonService>();
        containerRegistry.RegisterSingleton<ISmartHomeMockService, SmartHomeMockService>();
        containerRegistry.RegisterSingleton<IAqaraService, AqaraService>();
        containerRegistry.RegisterSingleton<INotificationsService, NotificationsService>();
        containerRegistry.RegisterSingleton<ICamerasService, CamerasService>();
        containerRegistry.RegisterSingleton<IScenariosService, ScenariosService>();
        containerRegistry.RegisterSingleton<IRoomsService, RoomsService>();
    }

    private static void OnAppStart(INavigationService navigationService)
    {
        navigationService.CreateBuilder()   
            .AddSegment<MainTabbedPageViewModel>()
            .Navigate(HandleErrors);
    }

    private static void HandleErrors(Exception exception)
    {
        Debug.WriteLine(exception.Message);
        Debugger.Break();
    }

    private static void OnConfigureMauiHandlers(IMauiHandlersCollection handlers)
    {
        handlers.AddCompatibilityRenderer(typeof(CustomTabbedPage), typeof(CustomTabbedPageRenderer));
    }

    #endregion
}

