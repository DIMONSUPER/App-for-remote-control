namespace SmartMirror.Effects.Touch.EventArgs
{
    public class TouchCompletedEventArgs : global::System.EventArgs
    {
        internal TouchCompletedEventArgs(object? parameter) => Parameter = parameter;

        public object? Parameter { get; }
    }
}
