namespace SmartMirror.Controls
{
    public class StickyHeaderCollectionView : CollectionView
    {
        public StickyHeaderCollectionView()
        {
            AppendToMapping();
        }

        #region -- Public properties --

        public static readonly BindableProperty HeaderPositionProperty = BindableProperty.Create(
            propertyName: nameof(HeaderPosition),
            returnType: typeof(int),
            declaringType: typeof(StickyHeaderCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public int HeaderPosition
        {
            get => (int)GetValue(HeaderPositionProperty);
            set => SetValue(HeaderPositionProperty, value);
        }

        #endregion

        #region -- Private helpers --

        private void AppendToMapping()
        {
            Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping(nameof(StickyHeaderCollectionView), (handler, view) =>
            {
                if (handler?.PlatformView is not null)
                {
                    handler.PlatformView.ScrollChange += OnScrollChanged;
                }
            });
        }

        private void OnScrollChanged(object sender, Android.Views.View.ScrollChangeEventArgs e)
        {
            if (sender is AndroidX.RecyclerView.Widget.RecyclerView recyclerView && recyclerView.GetChildAt(0) is Android.Views.View topChild)
            {
                int topChildPosition = recyclerView.GetChildAdapterPosition(topChild);

                if (topChildPosition != AndroidX.RecyclerView.Widget.RecyclerView.NoPosition)
                {
                    if (topChildPosition >= 0 && this.BindingContext is ViewModels.Tabs.Pages.NotificationsPageViewModel notificationsPageViewModel)
                    {
                        var headerPosition = 0;

                        for (int i = 0; topChildPosition >= 0 && i < notificationsPageViewModel.Notifications.Count; i++)
                        {
                            topChildPosition -= notificationsPageViewModel.Notifications[i].Count;
                            topChildPosition--;
                            headerPosition++;
                        }

                        HeaderPosition = headerPosition;
                    }
                }
            }
        }

        #endregion
    }
}
