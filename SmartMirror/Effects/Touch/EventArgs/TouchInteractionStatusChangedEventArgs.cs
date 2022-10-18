using SmartMirror.Effects.Touch.Enums;

namespace SmartMirror.Effects.Touch.EventArgs
{
    public class TouchInteractionStatusChangedEventArgs : global::System.EventArgs
    {
        internal TouchInteractionStatusChangedEventArgs(TouchInteractionStatus touchInteractionStatus)
            => TouchInteractionStatus = touchInteractionStatus;

        public TouchInteractionStatus TouchInteractionStatus { get; }
    }
}
