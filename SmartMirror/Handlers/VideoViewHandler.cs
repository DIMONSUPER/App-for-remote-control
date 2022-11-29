using Android.Content;
using Android.Runtime;
using Android.Views;
using LibVLCSharp.Shared;
using Microsoft.Maui.Handlers;
using SmartMirror.Controls;

namespace SmartMirror.Handlers;

public class VideoViewHandler : ViewHandler<VideoView, LibVLCSharp.Platforms.Android.VideoView>
{
    public VideoViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    #region -- Public properties --

    public static IPropertyMapper<VideoView, VideoViewHandler> PropertyMapper { get; set; } = new PropertyMapper<VideoView, VideoViewHandler>(ViewMapper)
    {
    };

    public static CommandMapper<VideoView, VideoViewHandler> CommandMapper { get; set; } = new(ViewCommandMapper)
    {
    };

    #endregion

    #region -- Overrides --

    protected override void ConnectHandler(LibVLCSharp.Platforms.Android.VideoView platformView)
    {
        base.ConnectHandler(platformView);

        VirtualView.MediaPlayerChanging += OnMediaPlayerChanging;
    }

    protected override void DisconnectHandler(LibVLCSharp.Platforms.Android.VideoView platformView)
    {
        VirtualView.MediaPlayerChanging -= OnMediaPlayerChanging;

        base.DisconnectHandler(platformView);
    }

    protected override LibVLCSharp.Platforms.Android.VideoView CreatePlatformView() => new(Context!);

    #endregion

    #region -- Private helpers --

    private void OnMediaPlayerChanging(object sender, MediaPlayerChangingEventArgs e)
    {
        if (PlatformView is not null)
        {
            PlatformView.MediaPlayer = e.NewMediaPlayer;
            PlatformView.TriggerLayoutChangeListener();
        }
    }

    #endregion
}
