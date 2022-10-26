using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System.Runtime.CompilerServices;

namespace SmartMirror.Controls
{
    public class CustomNoBorderEntry : Entry
    {
        public CustomNoBorderEntry()
        {
            AppendToMapping();
        }

        #region -- Public properties --

        public static readonly BindableProperty NeedToFocusProperty = BindableProperty.Create(
            propertyName: nameof(NeedToFocus),
            returnType: typeof(bool),
            declaringType: typeof(CustomNoBorderEntry));

        public bool NeedToFocus
        {
            get => (bool)GetValue(NeedToFocusProperty);
            set => SetValue(NeedToFocusProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(NeedToFocus))
            {
                Dispatcher.StartTimer(TimeSpan.FromMilliseconds(250), () =>
                {
                    Focus();

                    return false;
                }); 
            }
        }

        #endregion

        #region --- Private helpers ---

        private void AppendToMapping()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CustomNoBorderEntry", (handler, view) =>
            {
                if (view is CustomNoBorderEntry)
                {
                    var backgroundColor = BackgroundColor ?? Color.FromArgb("#ffffff");

                    handler.PlatformView.SetPadding(0, 0, 0, 0);
                    handler.PlatformView.SetBackgroundColor(backgroundColor.ToAndroid());
                }
            });
        }

        #endregion
    }
}
