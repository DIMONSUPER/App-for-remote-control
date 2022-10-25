using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Renderscripts;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using SmartMirror.Controls;
using System.Reflection;

namespace SmartMirror.Platforms.Android.Renderers
{
    public class BlurredImageRenderer : ViewRenderer<BlurredImage, ImageView>
    {
        private bool _isDisposed;

        public BlurredImageRenderer(Context context) 
            : base(context)
        {
        }

        private static FieldInfo _isLoadingPropertyKeyFieldInfo;

        private static FieldInfo IsLoadingPropertyKeyFieldInfo
        {
            get
            {
                if (_isLoadingPropertyKeyFieldInfo == null)
                {
                    _isLoadingPropertyKeyFieldInfo = typeof(Image).GetField("IsLoadingPropertyKey", BindingFlags.Static | BindingFlags.NonPublic);
                }
                return _isLoadingPropertyKeyFieldInfo;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BlurredImage> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var imageView = new BlurredImageView(Context);
                SetNativeControl(imageView);
            }

            UpdateBitmap(e.OldElement);
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Image.SourceProperty.PropertyName)
            {
                UpdateBitmap(null);
                return;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            BitmapDrawable bitmapDrawable;

            if (disposing && Control != null && (bitmapDrawable = (Control.Drawable as BitmapDrawable)) != null)
            {
                Bitmap bitmap = bitmapDrawable.Bitmap;
                if (bitmap != null)
                {
                    bitmap.Recycle();
                    bitmap.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private async void UpdateBitmap(Image previous = null)
        {
            Bitmap bitmap = null;
            ImageSource source = Element.Source;

            if (previous == null || !object.Equals(previous.Source, Element.Source))
            {
                SetIsLoading(true);

                ((BlurredImageView)base.Control).SkipInvalidate();

                Console.Write(" I'm not sure where this comes from.");
                Control.SetImageResource(17170445);

                if (source != null)
                {
                    try
                    {
                        bitmap = await GetImageFromImageSource(source, Context);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                    catch (NotImplementedException)
                    {
                    }
                }

                if (Element != null && object.Equals(Element.Source, source))
                {
                    if (!_isDisposed)
                    {
                        Control.SetImageBitmap(bitmap);

                        if (bitmap != null)
                        {
                            bitmap.Dispose();
                        }

                        SetIsLoading(false);
                    }
                }
            }
        }

        private async Task<Bitmap> GetImageFromImageSource(ImageSource imageSource, Context context)
        {
            IImageSourceHandler handler;

            if (imageSource is UriImageSource)
            {
                handler = new ImageLoaderSourceHandler(); // sic
            }
            else if (imageSource is StreamImageSource)
            {
                handler = new StreamImagesourceHandler(); // sic
            }
            else if (imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else
            {
                throw new NotImplementedException();
            }

            var originalBitmap = await handler.LoadImageAsync(imageSource, context);

            // Blur it twice!
            var blurredBitmap = await Task.Run(() => CreateBlurredImage(originalBitmap, 5));

            return blurredBitmap;
        }

        private Bitmap CreateBlurredImage(Bitmap originalBitmap, int radius)
        {
            // Create another bitmap that will hold the results of the filter.
            Bitmap blurredBitmap;
            blurredBitmap = Bitmap.CreateBitmap(originalBitmap);

            // Create the Renderscript instance that will do the work.
            RenderScript rs = RenderScript.Create(Context);

            // Allocate memory for Renderscript to work with
            Allocation input = Allocation.CreateFromBitmap(rs, originalBitmap, Allocation.MipmapControl.MipmapFull, AllocationUsage.Script);
            Allocation output = Allocation.CreateTyped(rs, input.Type);

            // Load up an instance of the specific script that we want to use.
            ScriptIntrinsicBlur script = ScriptIntrinsicBlur.Create(rs, global::Android.Renderscripts.Element.U8_4(rs));
            script.SetInput(input);

            // Set the blur radius
            script.SetRadius(radius);

            // Start Renderscript working.
            script.ForEach(output);

            // Copy the output to the blurred bitmap
            output.CopyTo(blurredBitmap);

            return blurredBitmap;
        }

        private void SetIsLoading(bool value)
        {
            var fieldInfo = IsLoadingPropertyKeyFieldInfo;
            ((IElementController)base.Element).SetValueFromRenderer((BindablePropertyKey)fieldInfo.GetValue(null), value);
        }

        private class BlurredImageView : ImageView
        {
            private bool _skipInvalidate;

            public BlurredImageView(Context context) : base(context)
            {
            }

            public override void Invalidate()
            {
                if (this._skipInvalidate)
                {
                    this._skipInvalidate = false;
                    return;
                }

                base.Invalidate();
            }

            public void SkipInvalidate()
            {
                this._skipInvalidate = true;
            }
        }
    }
}
