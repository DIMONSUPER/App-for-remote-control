using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using JP.Wasabeef.BlurryLib;
using Microsoft.Maui.Platform;
using SmartMirror.Services.Blur;
using static Android.Graphics.Bitmap;

namespace SmartMirror.Platforms.Android.Services
{
    public class BlurService : IBlurService
    {
        #region -- IBlurService implementation --

        public void BlurPopupBackground(int radius = 20)
        {
            var mainPage = App.Current.MainPage;
            var mainPageRenderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.Platform.GetRenderer(mainPage) as ViewGroup;

            if (mainPageRenderer is not null)
            {
                radius = (int)mainPageRenderer.Context.ToPixels(radius);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                {
                    var blurEffect = RenderEffect.CreateBlurEffect(radius, radius, Shader.TileMode.Clamp);
                    mainPageRenderer.SetRenderEffect(blurEffect);
                }
                else
                {
                    mainPageRenderer.Post(() =>
                    {
                        if (mainPage.Navigation.ModalStack.Any() && mainPage.Navigation.ModalStack[^1] is ContentPage currentPopup)
                        {
                            var byteArray = GetBlurredBackgroundBytes(mainPageRenderer, radius);
                            currentPopup.BackgroundImageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
                        }
                    });
                }
            }
        }

        public void UnblurPopupBackground()
        {
            var mainPage = App.Current.MainPage;
            var mainPageRenderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.Platform.GetRenderer(mainPage) as ViewGroup;

            if (mainPageRenderer is not null && Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                mainPageRenderer.SetRenderEffect(null);
            }
        }

        #endregion

        #region -- Private helpers --

        private byte[] GetBlurredBackgroundBytes(global::Android.Views.View view, int radius)
        {
            try
            {
                using var imageView = new ImageView(view.Context);

                Blurry
                    .With(view.Context)
                    .Radius(radius)
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
}

