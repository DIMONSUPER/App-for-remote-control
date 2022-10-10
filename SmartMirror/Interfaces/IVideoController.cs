using SmartMirror.Enums;

namespace SmartMirror.Interfaces
{
    public interface IVideoController
    {
        EVideoLoadingState LoadingState { get; set; }
    }
}
