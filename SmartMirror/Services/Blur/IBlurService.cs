namespace SmartMirror.Services.Blur;

public interface IBlurService
{
    void BlurPopupBackground(int radius = 20);

    void UnblurPopupBackground();
}

