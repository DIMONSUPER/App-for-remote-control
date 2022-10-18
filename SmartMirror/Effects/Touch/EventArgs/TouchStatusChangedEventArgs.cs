using SmartMirror.Effects.Touch.Enums;

namespace SmartMirror.Effects.Touch.EventArgs
{
    public class TouchStatusChangedEventArgs : global::System.EventArgs
    {
        internal TouchStatusChangedEventArgs(TouchStatus status) => Status = status;

        public TouchStatus Status { get; }
    }
}
