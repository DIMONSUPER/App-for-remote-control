namespace SmartMirror.Controls
{
    [ContentProperty("Tempalate")]
    public class LazyContentView : ContentView
    {
        private bool _isPageLoaded;

        #region -- Public properties --

        public DataTemplate Tempalate { get; set; }

        #endregion

        #region -- Public helpers --

        public void TryLoadView()
        {
            if (!_isPageLoaded)
            {
                try
                {
                    var content = Tempalate?.CreateContent();

                    if (content is View view)
                    {
                        Content = view;

                        _isPageLoaded = true;
                    }
                }
                catch
                {
                }

                if (Content is null)
                {
                    Content = new Label()
                    {
                        Text = "No set content",
                    };
                }
            }
        }

        #endregion
    }
}
