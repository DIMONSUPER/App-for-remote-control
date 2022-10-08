using SmartMirror.Enums;

namespace SmartMirror.Interfaces
{
    public interface IVideoController
    {
        EVideoLoadingStatus LoadingStatus { get; set; }
    }
}
