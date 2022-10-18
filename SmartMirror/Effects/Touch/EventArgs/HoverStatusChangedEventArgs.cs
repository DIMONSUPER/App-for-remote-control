using SmartMirror.Effects.Touch.Enums;

namespace SmartMirror.Effects.Touch.EventArgs
{
    public class HoverStatusChangedEventArgs : global::System.EventArgs
    {
        internal HoverStatusChangedEventArgs(HoverStatus status) => Status = status;

        public HoverStatus Status { get; }
    }
}
