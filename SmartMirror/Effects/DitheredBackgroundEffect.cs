using Microsoft.Maui.Controls.PlatformConfiguration;
using System.ComponentModel;

namespace SmartMirror.Effects
{
    public static class DitheredBackgroundEffect
    {
        #region -- Public properties --

        public static readonly BindableProperty IsDitheredProperty = BindableProperty.CreateAttached(
            propertyName: "IsDithered",
            returnType: typeof(bool),
            declaringType: typeof(VisualElement),
            defaultValue: false,
            propertyChanged: OnDitherPropertyChanged);

        #endregion

        #region -- Public helpers --

        public static bool GetIsDithered(BindableObject view) => (bool)view.GetValue(IsDitheredProperty);

        public static void SetIsDithered(BindableObject view, bool value) => view.SetValue(IsDitheredProperty, value);

        #endregion

        #region -- Private helpers --

        private static void OnDitherPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is VisualElement element)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
                {
                    SetBackgroundDithering(element);

                    element.PropertyChanged -= OnElementPropertyChanged;
                    element.PropertyChanged += OnElementPropertyChanged; 
                }
                else
                {
                    // TO DO: make background smoothing for SDK below 31
                }
            }
        }

        private static void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is VisualElement element && e.PropertyName is nameof(element.Background))
            {
                SetBackgroundDithering(element);
            }
        }

        private static void SetBackgroundDithering(VisualElement element)
        {
            var isBackgroundDithered = GetIsDithered(element);

            element.Dispatcher.Dispatch(() =>
            {
                if (element.Handler?.PlatformView is Android.Views.View view)
                {
                    view.Background?.SetDither(isBackgroundDithered);
                }
            });
        }

        #endregion
    }
}
