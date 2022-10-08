using SmartMirror.Enums;

namespace SmartMirror.Interfaces
{
    public interface IVideoController
    {
        EVideoStatus Status { get; set; }
    }
}
