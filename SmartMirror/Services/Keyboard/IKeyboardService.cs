namespace SmartMirror.Services.Keyboard;

public interface IKeyboardService
{
    event EventHandler KeyboardHeightChanged;

    double KeyboardHeight { get; }
}

