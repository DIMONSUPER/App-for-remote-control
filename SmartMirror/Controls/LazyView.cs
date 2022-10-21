namespace SmartMirror.Controls
{
    [ContentProperty("Template")]
    public class LazyView : ContentView
    {
        private bool _isPageLoaded;

        #region -- Public properties --
        
        public DataTemplate Template { get; set; }
        
        #endregion

        #region -- Public helpers --

        public void LoadView()
        {
            if (!_isPageLoaded)
            {
                Content = (View)Template.CreateContent();

                _isPageLoaded = true;
            }
        }

        #endregion
    }
}
