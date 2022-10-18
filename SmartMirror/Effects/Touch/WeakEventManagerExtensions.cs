namespace SmartMirror.Effects.Touch
{
    /// <summary>
    /// Extensions for Xamarin.Forms.WeakEventManager
    /// </summary>
    public static class WeakEventManagerExtensions
    {
        /// <summary>
        /// Invokes the event EventHandler
        /// </summary>
        /// <param name="weakEventManager">WeakEventManager</param>
        /// <param name="sender">Sender</param>
        /// <param name="eventArgs">Event arguments</param>
        /// <param name="eventName">Event name</param>
        public static void RaiseEvent(this WeakEventManager weakEventManager, object? sender, object eventArgs, string eventName)
        {
            _ = weakEventManager ?? throw new ArgumentNullException(nameof(weakEventManager));

            weakEventManager.HandleEvent(sender, eventArgs, eventName);
        }
    }
}
