using SmartMirror.Effects.Touch.Enums;

namespace SmartMirror.Effects.Touch.EventArgs
{
    public class HoverStateChangedEventArgs : global::System.EventArgs
    {
        internal HoverStateChangedEventArgs(HoverState state) => State = state;

        public HoverState State { get; }
    }
}
