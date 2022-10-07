using SmartMirror.Controls.Video;

namespace SmartMirror.Handlers
{
    public partial class VideoHandler
    {
        public VideoHandler() : base(PropertyMapper, CommandMapper)
        {
        }

        #region -- Public properties --

        public static IPropertyMapper<Video, VideoHandler> PropertyMapper = new PropertyMapper<Video, VideoHandler>(ViewMapper)
        {
            [nameof(Video.AreTransportControlsEnabled)] = MapAreTransportControlsEnabled,
            [nameof(Video.Source)] = MapSource,
            [nameof(Video.Position)] = MapPosition,
        };

        public static CommandMapper<Video, VideoHandler> CommandMapper = new(ViewCommandMapper)
        {
            [nameof(Video.UpdateStatus)] = MapUpdateStatus,
            [nameof(Video.PlayRequested)] = MapPlayRequested,
            [nameof(Video.PauseRequested)] = MapPauseRequested,
            [nameof(Video.StopRequested)] = MapStopRequested
        };

        #endregion
    }
}
