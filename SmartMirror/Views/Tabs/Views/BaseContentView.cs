namespace SmartMirror.Views.Tabs.Views
{
    public class BaseContentView : ContentView, IPageLifecycleAware
    {
        #region -- Public properties --

        public event EventHandler Appearing;

        public event EventHandler Disappearing;

        #endregion

        #region -- IPageLifecycleAware implementation --

        public void OnAppearing()
        {
            Appearing?.Invoke(this, EventArgs.Empty);
        }

        public void OnDisappearing()
        {
            Disappearing?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
