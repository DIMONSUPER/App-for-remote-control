using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;
using System.Runtime.CompilerServices;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace SmartMirror.Controls
{
    public class CustomNoBorderEntry : Entry
    {
        public CustomNoBorderEntry()
        {
            AppendToMapping();
        }

        #region --- Private helpers ---

        private void AppendToMapping()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(CustomNoBorderEntry), (handler, view) =>
            {
                if (view is CustomNoBorderEntry)
                {
                    var backgroundColor = BackgroundColor ?? Colors.Transparent;

                    handler.PlatformView?.SetPadding(0, 0, 0, 0);
                    handler.PlatformView?.SetBackgroundColor(backgroundColor.ToAndroid());
                }
            });
        }

        #endregion
    }
}
