using System.Collections.ObjectModel;
using SmartMirror.Models.BindableModels;
using SmartMirror.Interfaces;

namespace SmartMirror.Controls
{
    public class StickyHeaderCollectionView : CollectionView
    {
        private int _headerPosition;

        public StickyHeaderCollectionView()
        {
            AppendToMapping();
        }

        #region -- Public properties --

        public static readonly BindableProperty NameCurrentGroupProperty = BindableProperty.Create(
            propertyName: nameof(NameCurrentGroup),
            returnType: typeof(string),
            defaultValue: string.Empty,
            declaringType: typeof(StickyHeaderCollectionView),
            defaultBindingMode: BindingMode.OneWayToSource);

        public string NameCurrentGroup
        {
            get => (string)GetValue(NameCurrentGroupProperty);
            set => SetValue(NameCurrentGroupProperty, value);
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

                if (topChildPosition != AndroidX.RecyclerView.Widget.RecyclerView.NoPosition
                    && topChildPosition >= 0 && ItemsSource is ObservableCollection<IGroupableCollection> items)
                {
                    CalculateHeaderPosition(topChildPosition, items);

                    NameCurrentGroup = GetNameCurrentGroup(items);
                }
            }
        }

        private void CalculateHeaderPosition(int topChildPosition, ObservableCollection<IGroupableCollection> items)
        {
            var headerPosition = 0;

            for (int i = 0; topChildPosition >= 0 && i < items.Count; i++)
            {
                topChildPosition -= items[i].ItemsCount;
                topChildPosition--;
                headerPosition++;
            }

            _headerPosition = headerPosition;
        }

        private string GetNameCurrentGroup(ObservableCollection<IGroupableCollection> items)
        {
            string nameCurrentGroup = string.Empty;

            var arrayPosition = _headerPosition - 1;

            if (arrayPosition > -1 && arrayPosition < items.Count)
            {
                nameCurrentGroup = items[arrayPosition].GroupName;
            }

            return nameCurrentGroup;
        }

        #endregion
    }
}