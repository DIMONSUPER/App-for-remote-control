using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Dispatching;
using SmartMirror.Resources.Strings;

namespace SmartMirror.ViewModels;

public class MainTabbedPageViewModel : BaseViewModel
{
    private int _buttonCount;

    public MainTabbedPageViewModel(
        INavigationService navigationService)
        : base(navigationService)
    {
    }

    #region -- Overrides --

    public override bool OnBackButtonPressed()
    {
        if (_buttonCount < 1)
        {
            var interval = TimeSpan.FromMilliseconds(500);
            Application.Current.Dispatcher.StartTimer(interval, GetCountBackButtonPresses);
        }

        _buttonCount++;

        return true;
    }

    #endregion

    #region -- Private helpers --

    private bool GetCountBackButtonPresses()
    {
        if (_buttonCount > 1)
        {
            Application.Current.Quit();
        }
        else
        {
            Toast.Make(Strings.NeedsTwoTaps).Show();
        }

        _buttonCount = 0;

        return false;
    }

    #endregion
}

