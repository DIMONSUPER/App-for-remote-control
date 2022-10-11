namespace SmartMirror.TriggerActions;

public class ScrollCollectionViewEventAction : TriggerAction<VisualElement>
{
    #region -- Public properties --

    public CollectionView CollectionView { get; set; }

    public int ItemIndex { get; set; }

    public int GroupIndex { get; set; } = -1;

    public ScrollToPosition ScrollPosition { get; set; } = ScrollToPosition.Start;

    public bool IsAnimated { get; set; }

    public int IntervalInMs { get; set; } = 150;

    #endregion

    #region -- Overrides --

    protected override void Invoke(VisualElement sender)
    {
        if (CollectionView is not null)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(IntervalInMs), () =>
            {
                CollectionView.ScrollTo(ItemIndex, GroupIndex, ScrollPosition, IsAnimated);

                return false;
            });
        }
    }

    #endregion
}
