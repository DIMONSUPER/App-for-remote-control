using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Localization;
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
            TimeSpan timer = new TimeSpan(0, 0, 0, 0, 500);
            Device.StartTimer(timer, GetCountBackButtonPresses);
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

