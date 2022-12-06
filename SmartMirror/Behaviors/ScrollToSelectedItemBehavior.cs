using System.Runtime.CompilerServices;

namespace SmartMirror.Behaviors
{
    public class ScrollToSelectedItemBehavior : Behavior<CollectionView>
    {
        private CollectionView _collectionView;

        #region -- Public properties --
        
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(object),
            declaringType: typeof(ScrollToSelectedItemBehavior),
            defaultBindingMode: BindingMode.OneWay);

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public ScrollToPosition ScrollToPosition { get; set; } = ScrollToPosition.Center;

        #endregion

        #region -- Overrides --

        protected override void OnAttachedTo(CollectionView bindable)
        {
            base.OnAttachedTo(bindable);

            _collectionView = bindable;
        }

        protected override void OnDetachingFrom(CollectionView bindable)
        {
            _collectionView = null;

            base.OnDetachingFrom(bindable);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(SelectedItem) && SelectedItem is not null)
            {
                _collectionView.ScrollTo(SelectedItem, null, ScrollToPosition, false);
            }
        } 

        #endregion
    }
}
