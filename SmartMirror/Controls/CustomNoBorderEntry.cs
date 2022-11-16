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

            Focused += OnFocusChanged;
            Unfocused += OnFocusChanged;
        }

        #region -- Public properties --

        public static readonly BindableProperty IsEntryFocusedProperty = BindableProperty.Create(
            propertyName: nameof(IsEntryFocused),
            returnType: typeof(bool),
            declaringType: typeof(CustomNoBorderEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsEntryFocused
        {
            get => (bool)GetValue(IsEntryFocusedProperty);
            set => SetValue(IsEntryFocusedProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsEntryFocusedProperty.PropertyName)
            {
                var needToDisableFullscreenWhileKeyboardDisplayed = Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.R;

                try
                {
                    if (IsEntryFocused && needToDisableFullscreenWhileKeyboardDisplayed)
                    {
                        Platform.CurrentActivity.Window.ClearFlags(WindowManagerFlags.Fullscreen);
                    }
                    else if (!IsEntryFocused && needToDisableFullscreenWhileKeyboardDisplayed)
                    {
                        Platform.CurrentActivity.Window.AddFlags(WindowManagerFlags.Fullscreen);
                    }

                    if (this.Handler is EntryHandler entryHandler)
                    {
                        UpdateFocus(entryHandler);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        #endregion

        #region --- Private helpers ---

        private void OnFocusChanged(object sender, FocusEventArgs e)
        {
            IsEntryFocused = e.IsFocused;
        }

        private void AppendToMapping()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(CustomNoBorderEntry), (handler, view) =>
            {
                if (view is CustomNoBorderEntry)
                {
                    var backgroundColor = BackgroundColor ?? Color.FromArgb("#ffffff");

                    handler.PlatformView?.SetPadding(0, 0, 0, 0);
                    handler.PlatformView?.SetBackgroundColor(backgroundColor.ToAndroid());
                }
            });
        }

        private void UpdateFocus(EntryHandler handler)
        {
            var editText = handler?.PlatformView;

            if (editText is not null && Platform.CurrentActivity?.GetSystemService(Context.InputMethodService) is InputMethodManager inputMethodManager)
            {
                if (IsEntryFocused && editText.Focusable)
                {
                    editText.RequestFocus();
                    inputMethodManager.ShowSoftInput(editText, Android.Views.InputMethods.ShowFlags.Forced);
                }
                else
                {
                    Unfocus();
                    inputMethodManager.HideSoftInputFromWindow(editText.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
                }
            }
        }

        #endregion
    }
}
