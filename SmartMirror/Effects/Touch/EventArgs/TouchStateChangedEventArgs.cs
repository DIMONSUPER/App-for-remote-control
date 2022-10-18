using SmartMirror.Effects.Touch.Enums;

namespace SmartMirror.Effects.Touch.EventArgs
{
    public class TouchStateChangedEventArgs : global::System.EventArgs
    {
        internal TouchStateChangedEventArgs(TouchState state)
            => State = state;

        public TouchState State { get; }
    }
}
