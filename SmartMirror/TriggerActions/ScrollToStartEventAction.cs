namespace SmartMirror.TriggerActions;

public class ScrollToStartEventAction : TriggerAction<VisualElement>
{
    #region -- Public properties --

    public VisualElement View { get; set; }

    public bool IsAnimated { get; set; }

    public int IntervalMilliseconds { get; set; } = 150;

    #endregion

    #region -- Overrides --

    protected override void Invoke(VisualElement sender)
    {
        View?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(IntervalMilliseconds), OnTimerCallback);
    }

    #endregion

    #region -- Private helpers --

    private bool OnTimerCallback()
    {
        if (View is ScrollView scrollView)
        {
            scrollView.ScrollToAsync(0, 0, IsAnimated);
        }
        else if (View is CollectionView colllectionVIew)
        {
            colllectionVIew.ScrollTo(0, -1, ScrollToPosition.Start, IsAnimated);
        }

        return false;
    }

    #endregion
}
