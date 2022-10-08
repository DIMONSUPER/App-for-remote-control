using Microsoft.Maui.Handlers;
using SmartMirror.Controls;
using SmartMirror.Helpers;
using SmartMirror.Platforms.Android.Controls;

namespace SmartMirror.Handlers
{
    public partial class VideoHandler : ViewHandler<Video, MauiVideoPlayer>
    {
        #region -- Overrides --

        protected override MauiVideoPlayer CreatePlatformView() => new MauiVideoPlayer(Context, VirtualView);

        protected override void ConnectHandler(MauiVideoPlayer platformView)
        {
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(MauiVideoPlayer platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        #endregion

        #region -- Public helpers --
        
        public static void MapIsControlPanelEnabled(VideoHandler handler, Video video)
        {
            handler.PlatformView?.UpdateControlPanelEnabled();
        }

        public static void MapSource(VideoHandler handler, Video video)
        {
            handler.PlatformView?.TryUpdateSource();
        }

        public static void MapPosition(VideoHandler handler, Video video)
        {
            handler.PlatformView?.UpdatePosition();
        }

        public static void MapUpdateStatus(VideoHandler handler, Video video, object? args)
        {
            handler.PlatformView?.UpdateStatus();
        }

        public static void MapPlayRequested(VideoHandler handler, Video video, object? args)
        {
            if (args is VideoPositionEventArgs videoPositionEventArgs)
            {
                handler.PlatformView?.PlayRequested(videoPositionEventArgs.Position);
            }
        }

        public static void MapPauseRequested(VideoHandler handler, Video video, object? args)
        {
            if (args is VideoPositionEventArgs videoPositionEventArgs)
            {
                handler.PlatformView?.PauseRequested(videoPositionEventArgs.Position);
            }
        }

        public static void MapStopRequested(VideoHandler handler, Video video, object? args)
        {
            if (args is VideoPositionEventArgs videoPositionEventArgs)
            {
                handler.PlatformView?.StopRequested(videoPositionEventArgs.Position);
            }
        }

        #endregion
    }
}