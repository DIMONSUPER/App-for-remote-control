using Microsoft.Maui.Controls.Compatibility.Platform.Android;
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
            declaringType: typeof(CustomNoBorderEntry));

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
                if (IsEntryFocused)
                {
                    Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(250), () => this.Focus());
                }
                else
                {
                    this.Unfocus();
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

                    handler.PlatformView.SetPadding(0, 0, 0, 0);
                    handler.PlatformView.SetBackgroundColor(backgroundColor.ToAndroid());
                }
            });

            Entry.ControlsEntryMapper.AppendToMapping(nameof(Entry.IsFocused), UpdateFocus);
        }

        private void UpdateFocus(EntryHandler handler, IEntry entry)
        {
            var editText = handler.PlatformView;

            if (Platform.CurrentActivity.GetSystemService(Android.Content.Context.InputMethodService) is Android.Views.InputMethods.InputMethodManager inputMethodManager)
            {
                if (IsFocused)
                {
                    editText.RequestFocus();
                    inputMethodManager.ShowSoftInput(editText, Android.Views.InputMethods.ShowFlags.Forced);
                }
                else
                {
                    inputMethodManager.HideSoftInputFromWindow(editText.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
                }
            }
        }

        #endregion
    }
}
