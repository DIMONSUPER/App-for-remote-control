using System;

namespace SmartMirror.TriggerActions
{
    public class UnfocusTriggerAction : TriggerAction<VisualElement>
    {
        #region -- Public properties --

        public VisualElement View { get; set; }

        #endregion

        #region -- Overrides --

        protected override void Invoke(VisualElement sender)
        {
            if (View is not null)
            {
                View.IsEnabled = false;
                View.IsEnabled = true;
            }
        }

        #endregion
    }
}