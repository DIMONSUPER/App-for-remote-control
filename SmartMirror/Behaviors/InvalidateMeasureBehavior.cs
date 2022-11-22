using System.Runtime.CompilerServices;

namespace SmartMirror.Behaviors
{
    public class InvalidateMeasureBehavior : Behavior<VisualElement>
    {
        private IView _view;

        #region -- Public properties --

        public static readonly BindableProperty TrackedValueProperty = BindableProperty.Create(
            propertyName: nameof(TrackedValue),
            returnType: typeof(object),
            declaringType: typeof(InvalidateMeasureBehavior),
            defaultBindingMode: BindingMode.OneWay);

        public object TrackedValue
        {
            get => GetValue(TrackedValueProperty);
            set => SetValue(TrackedValueProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnAttachedTo(VisualElement bindable)
        {
            _view = bindable;

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            _view = null;

            base.OnDetachingFrom(bindable);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(TrackedValue))
            {
                _view?.InvalidateMeasure();
            }
        } 

        #endregion
    }
}