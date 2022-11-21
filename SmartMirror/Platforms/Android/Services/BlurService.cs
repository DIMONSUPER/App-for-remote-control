using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using JP.Wasabeef.BlurryLib;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Platform;
using SmartMirror.Services.Blur;
using static Android.Graphics.Bitmap;
using Color = Microsoft.Maui.Graphics.Color;
using AColor = Android.Graphics.Color;

namespace SmartMirror.Platforms.Android.Services;

public class BlurService : IBlurService
{
    #region -- IBlurService implementation --

    public void BlurPopupBackground(Color color, int radius = 20)
    {
        var currentPage = GetCurrentPage();

        //var currentPageNativeView = currentPage.Handler?.PlatformView as global::Android.Views.View;
        var currentPageNativeView = currentPage.Handler.MauiContext.Services.GetRequiredService<NavigationRootManager>().RootView;

        if (currentPageNativeView is not null)
        {   
            radius = (int)currentPageNativeView.Context.ToPixels(radius);
            var androidColor = color.ToAndroid();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                currentPageNativeView.Post(() =>
                {
                    var colorFilter = new PorterDuffColorFilter(androidColor, PorterDuff.Mode.SrcAtop);

                    var blurEffect = RenderEffect.CreateBlurEffect(radius, radius, Shader.TileMode.Clamp);
                    var colorEffect = RenderEffect.CreateColorFilterEffect(colorFilter, blurEffect);

                    currentPageNativeView.SetRenderEffect(colorEffect);
                });
            }
            else
            {
                currentPageNativeView.Post(() =>
                {
                    if (currentPage.Navigation.ModalStack.Any() && currentPage.Navigation.ModalStack[^1] is ContentPage currentPopup)
                    {
                        var byteArray = GetBlurredBackgroundBytes(currentPageNativeView, radius, androidColor);
                        currentPopup.BackgroundImageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
                    }
                });
            }
        }
    }

    public void UnblurPopupBackground()
    {
        var currentPage = GetCurrentPage();

        //var currentPageNativeView = currentPage.Handler?.PlatformView as global::Android.Views.View;
        var currentPageNativeView = currentPage.Handler.MauiContext.Services.GetRequiredService<NavigationRootManager>().RootView;

        if (currentPageNativeView is not null && Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            currentPageNativeView.SetRenderEffect(null);
        }
    }

    #endregion

    #region -- Private helpers --

    private Page GetCurrentPage()
    {
        var currentPage = App.Current.MainPage;

        if (currentPage.Navigation.ModalStack.Count != 0)
        {
            currentPage = currentPage.Navigation.ModalStack[^1];
        }

        return currentPage;
    }

    private byte[] GetBlurredBackgroundBytes(global::Android.Views.View view, int radius, AColor color)
    {
        try
        {
            using var imageView = new ImageView(view.Context);

            Blurry
                .With(view.Context)
                .Radius(radius)
                .Color(color)
                .Capture(view)
                .Into(imageView);

            using var bitmap = (imageView?.Drawable as BitmapDrawable).Bitmap;

            using var stream = new MemoryStream();

            bitmap?.Compress(CompressFormat.Jpeg, 50, stream);

            return stream?.ToArray();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(GetBlurredBackgroundBytes)}: {ex.Message}");

            return Array.Empty<byte>();
        }
    }

    #endregion
}