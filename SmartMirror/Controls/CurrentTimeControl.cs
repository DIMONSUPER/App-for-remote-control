namespace SmartMirror.Controls
{
    public class CurrentTimeControl : Label
    {
        public CurrentTimeControl()
        {
            Text = DateTime.Now.ToString("HH:mm");

            Application.Current.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Text = DateTime.Now.ToString("HH:mm");
                return true;
            });
        }
    }
}

