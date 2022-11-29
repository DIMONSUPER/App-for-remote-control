using LibVLCSharp.Shared;

namespace SmartMirror.Controls;

public class VideoView : View
{
    public event EventHandler<MediaPlayerChangingEventArgs> MediaPlayerChanging;

    public event EventHandler<MediaPlayerChangedEventArgs> MediaPlayerChanged;

    public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer),
            typeof(LibVLCSharp.Shared.MediaPlayer),
            typeof(VideoView),
            propertyChanging: OnMediaPlayerChanging,
            propertyChanged: OnMediaPlayerChanged);

    public LibVLCSharp.Shared.MediaPlayer MediaPlayer
    {
        get => GetValue(MediaPlayerProperty) as LibVLCSharp.Shared.MediaPlayer;
        set => SetValue(MediaPlayerProperty, value);
    }

    #region -- Private helpers --

    private static void OnMediaPlayerChanging(BindableObject bindable, object oldValue, object newValue)
    {
        var videoView = (VideoView)bindable;
        videoView.MediaPlayerChanging?.Invoke(videoView, new MediaPlayerChangingEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
    }

    private static void OnMediaPlayerChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var videoView = (VideoView)bindable;
        videoView.MediaPlayerChanged?.Invoke(videoView, new MediaPlayerChangedEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
    }

    #endregion
}

