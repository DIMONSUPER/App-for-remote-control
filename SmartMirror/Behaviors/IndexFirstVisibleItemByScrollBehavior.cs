using System;
using System.Runtime.CompilerServices;

namespace SmartMirror.Behaviors
{
    public class IndexFirstVisibleItemByScrollBehavior : Behavior<CollectionView>
    {
        #region -- Public properties --

        public static readonly BindableProperty ItemIndexProperty = BindableProperty.Create(
            propertyName: nameof(ItemIndex),
            returnType: typeof(int),
            declaringType: typeof(IndexFirstVisibleItemByScrollBehavior),
            defaultBindingMode: BindingMode.OneWayToSource);

        public int ItemIndex
        {
            get => (int)GetValue(ItemIndexProperty);
            set => SetValue(ItemIndexProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnAttachedTo(CollectionView bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.Scrolled += OnScrolled;
        }

        protected override void OnDetachingFrom(CollectionView bindable)
        {
            if (bindable is not null)
            {
                bindable.Scrolled -= OnScrolled;
            }

            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region -- Private helpers --

        private void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            ItemIndex = e.CenterItemIndex;
        }

        #endregion
    }
}

