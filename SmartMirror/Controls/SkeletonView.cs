namespace SmartMirror.Controls;

public class SkeletonView : Border
{
	public SkeletonView()
	{
        Dispatcher.StartTimer(TimeSpan.FromSeconds(1.5), () =>
        {
            this.FadeTo(0.5, 750, Easing.CubicInOut).ContinueWith((x) =>
            {
                this.FadeTo(1, 750, Easing.CubicInOut);
            });

            return true;
        });
    }
}
