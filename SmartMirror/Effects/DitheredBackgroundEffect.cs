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
                var isBackgroundDithered = GetIsDithered(bindable);

                bindable.Dispatcher.Dispatch(() =>
                {
                    if (element.Handler?.PlatformView is Android.Views.View view)
                    {
                        view.Background?.SetDither(isBackgroundDithered);
                    }
                });
            }
        } 

        #endregion
    }
}
