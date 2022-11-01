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
            Label.ControlsLabelMapper.AppendToMapping(nameof(Label.MaxLines), UpdateMaxLines);
            Label.ControlsLabelMapper.AppendToMapping(nameof(Label.LineBreakMode), UpdateMaxLines);
        }

        private void UpdateMaxLines(Microsoft.Maui.Handlers.LabelHandler handler, ILabel label)
        {
            var textView = handler.PlatformView;
            if (label is Label controlsLabel && textView.Ellipsize == Android.Text.TextUtils.TruncateAt.End)
            {
                textView.SetMaxLines(controlsLabel.MaxLines);
            }
        }

        #endregion
    }
}
