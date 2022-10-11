namespace SmartMirror.TriggerActions;

public class ScrollScrollViewEventAction : TriggerAction<VisualElement>
{
    #region -- Public properties --

    public ScrollView ScrollView { get; set; }

    public double ScrollX { get; set; }

    public double ScrollY { get; set; }

    public bool IsAnimated { get; set; }

    public int IntervalInMs { get; set; } = 150;

    #endregion

    #region -- Overrides --

    protected override void Invoke(VisualElement sender)
    {
        if (ScrollView is not null)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(IntervalInMs), () =>
            {
                ScrollView.ScrollToAsync(ScrollX, ScrollY, IsAnimated);

                return false;
            });
        }
    }

    #endregion
}
