namespace SmartMirror.Effects.Touch.EventArgs
{
    public class LongPressCompletedEventArgs : global::System.EventArgs
    {
        internal LongPressCompletedEventArgs(object? parameter) => Parameter = parameter;

        public object? Parameter { get; }
    }
}
