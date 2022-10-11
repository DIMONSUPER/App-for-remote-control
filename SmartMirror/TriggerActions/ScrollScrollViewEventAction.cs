using SmartMirror.Views.Tabs;

namespace SmartMirror.TriggerActions
{
    public class ScrollScrollViewEventAction : TriggerAction<ContentPage>
    {
        #region -- Public properties --

        public ScrollView ScrollView { get; set; }

        public double ScrollX { get; set; }

        public double ScrollY { get; set; }

        public bool IsAnimated { get; set; }

        public int IntervalInMs { get; set; } = 150;

        #endregion

        #region -- Overrides --

        protected override void Invoke(ContentPage sender)
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
}
