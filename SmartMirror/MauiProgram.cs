using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using SmartMirror.Controls;
using SmartMirror.Handlers;
using SmartMirror.Platforms.Android.Renderers;
using SmartMirror.Platforms.Android.Services;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Blur;
//using SmartMirror.Platforms.Services;
//using SmartMirror.Services.Amazon;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Keyboard;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using SmartMirror.Services.Notifications;
using SmartMirror.Services.Repository;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Rooms;
using SmartMirror.Services.Scenarios;
using SmartMirror.Services.Settings;
using SmartMirror.ViewModels;
using SmartMirror.Views;
using SmartMirror.Views.Dialogs;
using SmartMirror.Views.Tabs.Details;
using SmartMirror.Views.Tabs.Pages;
using System.Diagnostics;

namespace SmartMirror;

public static class MauiProgram
{
    private static bool _isAuthorized;

    #region -- Public static helpers --

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCompatibility()
            .ConfigureMauiHandlers(OnConfigureMauiHandlers)
            .UsePrism(prism => prism.RegisterTypes(RegisterTypes).OnInitialized(OnInitialized).OnAppStart(OnAppStart))
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Medium-500.ttf", "InterMedium");
                fonts.AddFont("Inter-SemiBold-600.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold-700.ttf", "InterBold");
            });

        builder.Services.AddLocalization();

        return builder.Build();
    }

    #endregion

    #region -- Private static helpers --

    private static void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterDialog<ErrorDialog>();
        containerRegistry.RegisterDialog<EnterCodeDialog>();

        containerRegistry.RegisterForNavigation<SplashScreenPage>();
        containerRegistry.RegisterForNavigation<WelcomePage>();
        containerRegistry.RegisterForNavigation<MainTabbedPage>();
        containerRegistry.RegisterForNavigation<RoomsPage>();
        containerRegistry.RegisterForNavigation<NotificationsPage>();
        containerRegistry.RegisterForNavigation<CamerasPage>();
        containerRegistry.RegisterForNavigation<ScenariosPage>();
        containerRegistry.RegisterForNavigation<RoomDetailsPage>();
        containerRegistry.RegisterForNavigation<ScenarioDetailsPage>();

        containerRegistry.RegisterSingleton<IMapperService, MapperService>();
        containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
        containerRegistry.RegisterSingleton<IRestService, RestService>();
        containerRegistry.RegisterSingleton<IRepositoryService, RepositoryService>();
        containerRegistry.RegisterSingleton<IBlurService, BlurService>();
        //TODO: Remove when companion app is ready
        //containerRegistry.RegisterSingleton<IAmazonService, AmazonService>();
        containerRegistry.RegisterSingleton<ISmartHomeMockService, SmartHomeMockService>();
        containerRegistry.RegisterSingleton<IAqaraService, AqaraService>();
        containerRegistry.RegisterSingleton<IAqaraMessanger, AqaraMessanger>();
        containerRegistry.RegisterSingleton<INotificationsService, NotificationsService>();
        containerRegistry.RegisterSingleton<ICamerasService, CamerasService>();
        containerRegistry.RegisterSingleton<IScenariosService, ScenariosService>();
        containerRegistry.RegisterSingleton<IDevicesService, DevicesService>();
        containerRegistry.RegisterSingleton<IRoomsService, RoomsService>();
        containerRegistry.RegisterSingleton<IKeyboardService, KeyboardService>();
    }

    private static void OnInitialized(IContainerProvider container)
    {
        var aqaraService = container.Resolve<IAqaraService>();

        _isAuthorized = aqaraService.IsAuthorized;
    }

    private static void OnAppStart(INavigationService navigationService)
    {
        var navigationBuilder = navigationService.CreateBuilder();

        if (_isAuthorized)
        {
            navigationBuilder.AddSegment<MainTabbedPageViewModel>();
        }
        else
        {
            navigationBuilder.AddSegment<WelcomePageViewModel>();
        }

        navigationBuilder.Navigate(HandleErrors);
    }

    private static void HandleErrors(Exception exception)
    {
        Debug.WriteLine(exception.Message);
        Debugger.Break();
    }

    private static void OnConfigureMauiHandlers(IMauiHandlersCollection handlers)
    {
        handlers.AddHandler(typeof(Video), typeof(VideoHandler));
        handlers.AddCompatibilityRenderer(typeof(CustomTabbedPage), typeof(CustomTabbedPageRenderer));
    }

    #endregion
}

