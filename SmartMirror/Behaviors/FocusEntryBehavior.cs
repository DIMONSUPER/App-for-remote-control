using Android.Content;
using Android.Views.InputMethods;
using Android.Widget;
using Microsoft.Maui.Handlers;

namespace SmartMirror.Behaviors;

public class FocusEntryBehavior : Behavior<Entry>
{
    #region -- Private helpers --

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);

        entry.Loaded += OnEntryLoaded;
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.Completed -= OnEntryLoaded;

        base.OnDetachingFrom(entry);
    }

    #endregion

    #region -- Private helpers --

    private void OnEntryLoaded(object sender, EventArgs e)
    {
        if (sender is Entry entry && entry.Handler is EntryHandler entryHandler)
        {
            if (Platform.CurrentActivity?.GetSystemService(Context.InputMethodService) is InputMethodManager inputMethodManager)
            {
                entryHandler.PlatformView?.RequestFocus();
                inputMethodManager.ShowSoftInput(entryHandler.PlatformView, Android.Views.InputMethods.ShowFlags.Implicit);
            }
        }
    }

    #endregion
}

