using System;
namespace SmartMirror.Behaviors
{
    public class UnfocusableBehavior : Behavior<VisualElement>
    {
        private VisualElement _visualElement;

        #region -- Overrides --

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);

            _visualElement = bindable;
            _visualElement.Focused += OnVisualElementFocused;
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            _visualElement.Focused -= OnVisualElementFocused;
            _visualElement = null;

            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region -- Private helpers --

        private void OnVisualElementFocused(object sender, FocusEventArgs e)
        {
            if (_visualElement is not null)
            {
                _visualElement.Unfocus();
                _visualElement.IsEnabled = false;
                _visualElement.IsEnabled = true;
            }
        }

        #endregion
    }
}