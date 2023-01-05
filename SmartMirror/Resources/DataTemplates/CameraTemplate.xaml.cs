using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;

namespace SmartMirror.Resources.DataTemplates;

public partial class CameraTemplate : Grid
{
    private CameraBindableModel _currentBindingContext;

    public CameraTemplate()
    {
        InitializeComponent();
    }

    #region -- Overrides --

    protected override void OnBindingContextChanged()
    {
        if (_currentBindingContext is not null)
        {
            _currentBindingContext.PropertyChanged -= OnBindingContextPropertyChanged;
        }

        base.OnBindingContextChanged();

        if (BindingContext is CameraBindableModel cameraBindableModel)
        {
            _currentBindingContext = cameraBindableModel;
            _currentBindingContext.PropertyChanged += OnBindingContextPropertyChanged;
            UpdateAppearence();
        }
    }

    #endregion

    #region -- Private helpers --

    private void OnBindingContextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_currentBindingContext is not null && (e.PropertyName is nameof(CameraBindableModel.IsConnected) or nameof(CameraBindableModel.IsSelected)))
        {
            UpdateAppearence();
        }
    }

    private void UpdateAppearence()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            cameraStatusLabel.Text = _currentBindingContext.IsConnected ? Strings.Strings.On : Strings.Strings.Off;

            videoIcon.Source = GetVideoIconSource(_currentBindingContext.IsConnected, _currentBindingContext.IsSelected);

            cameraStatusLabel.TextColor = GetCameraStatusTextColor(_currentBindingContext.IsConnected, _currentBindingContext.IsSelected);

            cameraNameLabel.TextColor = _currentBindingContext.IsSelected ? Color.FromArgb("#252525") : Color.FromArgb("#FFFFFF");

            mainBorder.BackgroundColor = _currentBindingContext.IsSelected ? Color.FromArgb("#EAEAEA") : Colors.Transparent;
        });
    }

    private Color GetCameraStatusTextColor(bool isConnected, bool isSelected)
    {
        Color result;

        if (isSelected)
        {
            result = Color.FromArgb("#525252");
        }
        else
        {
            result = isConnected ? Color.FromArgb("#7EE5EB") : Color.FromArgb("#969697");
        }

        return result;
    }

    private string GetVideoIconSource(bool isConnected, bool isSelected)
    {
        string result;

        if (isConnected)
        {
            result = isSelected ? IconsNames.video_fill_dark : IconsNames.video_fill;
        }
        else
        {
            result = isSelected ? IconsNames.video_off_fill : IconsNames.video_off_fill_light;
        }

        return result;
    }

    #endregion
}