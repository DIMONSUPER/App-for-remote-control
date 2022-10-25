using Android.Content;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;

namespace SmartMirror.Platforms.Android.Renderers
{
    public class CustomNoBorderEntryRenderer : EntryRenderer
    {
        public CustomNoBorderEntryRenderer(Context context)
            : base(context)
        {
        }

        #region --- Overrides ---

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var backgroundColor = Element.BackgroundColor ?? Color.FromHex("#fff");

            if (Control is not null)
            {
                Control.SetPadding(0, 0, 0, 0);
                Control.SetBackgroundColor(backgroundColor.ToAndroid());
            }
        }

        #endregion

    }
}
