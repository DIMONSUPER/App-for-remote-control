using SmartMirror.Enums;

namespace SmartMirror.Interfaces
{
    public interface IVideoController
    {
        EVideoStatus Status { get; set; }

        TimeSpan Duration { get; set; }
    }
}
