using SmartMirror.Controls;

namespace SmartMirror.Handlers
{
    public partial class VideoHandler
    {
        public VideoHandler() : base(PropertyMapper, CommandMapper)
        {
        }

        #region -- Public properties --

        public static IPropertyMapper<Video, VideoHandler> PropertyMapper { get; set; } = new PropertyMapper<Video, VideoHandler>(ViewMapper)
        {
            [nameof(Video.Source)] = MapSource,
        };

        public static CommandMapper<Video, VideoHandler> CommandMapper { get; set; } = new(ViewCommandMapper)
        {
            [nameof(Video.PlayRequested)] = MapPlayRequested,
            [nameof(Video.PauseRequested)] = MapPauseRequested,
            [nameof(Video.StopRequested)] = MapStopRequested
        };

        #endregion
    }
}
