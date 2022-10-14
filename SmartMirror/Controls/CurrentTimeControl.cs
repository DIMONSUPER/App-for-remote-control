using System;
namespace SmartMirror.Controls
{
    public class CurrentTimeControl : Label
    {
        public CurrentTimeControl()
        {
            Application.Current.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Text = DateTime.Now.ToShortTimeString();
                return true;
            });
        }
    }
}

