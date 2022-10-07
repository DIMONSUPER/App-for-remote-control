namespace SmartMirror.Helpers
{
    public class VideoPositionEventArgs : EventArgs
    {
        #region -- Public properties --

        public TimeSpan Position { get; private set; }

        #endregion

        public VideoPositionEventArgs(TimeSpan timeSpan) => Position = timeSpan;
    }
}
