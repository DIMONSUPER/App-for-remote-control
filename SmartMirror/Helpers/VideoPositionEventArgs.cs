namespace SmartMirror.Helpers
{
    public class VideoPositionEventArgs : EventArgs
    {

        public VideoPositionEventArgs(TimeSpan timeSpan) => Position = timeSpan;

        #region -- Public properties --

        public TimeSpan Position { get; private set; }

        #endregion
    }
}
