using Microsoft.Maui.Handlers;

namespace SmartMirror.Controls
{
    public class CustomLabel : Label
    {
        public CustomLabel()
        {
            AppendToMapping();
        }

        #region -- Private helpers --

        private void AppendToMapping()
        {
            ControlsLabelMapper.AppendToMapping(nameof(MaxLines), UpdateMaxLines);
            ControlsLabelMapper.AppendToMapping(nameof(LineBreakMode), UpdateMaxLines);
        }

        private void UpdateMaxLines(LabelHandler handler, ILabel label)
        {
            var textView = handler.PlatformView;

            if (label is Label controlsLabel && controlsLabel.MaxLines > -1 && textView is not null && textView.Ellipsize == Android.Text.TextUtils.TruncateAt.End)
            {
                textView.SetMaxLines(controlsLabel.MaxLines);
            }
        }

        #endregion
    }
}
