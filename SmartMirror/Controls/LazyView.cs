namespace SmartMirror.Controls
{
    public class LazyView<TView> : ContentView where TView : View, new ()
    {
        private bool _isPageLoaded;

        #region -- Public helpers --

        public void TryLoadView()
        {
            if (!_isPageLoaded)
            {
                Content = new TView { BindingContext = BindingContext };

                _isPageLoaded = true;
            }
        }

        #endregion
    }
}
