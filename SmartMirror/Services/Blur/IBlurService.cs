namespace SmartMirror.Services.Blur;

public interface IBlurService
{
    void BlurPopupBackground(Color color, int radius = 20);

    void UnblurPopupBackground();
}

