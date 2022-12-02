using System;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using ShapeDrawable = Android.Graphics.Drawables.ShapeDrawable;
using Android.Graphics.Drawables.Shapes;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using static Android.Media.MediaParser;
using Paint = Android.Graphics.Paint;
using Microsoft.Maui.Handlers;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace SmartMirror.Controls
{
    public class CustomSlider : Slider
    {
        public CustomSlider()
        {
            AppendToMapping();
        }

        #region -- Public properties --

        public static readonly BindableProperty RadiusLineProperty = BindableProperty.Create(
            propertyName: nameof(RadiusLine),
            returnType: typeof(int),
            declaringType: typeof(CustomSlider),
            defaultValue: 4,
            defaultBindingMode: BindingMode.OneWay);

        public int RadiusLine
        {
            get => (int)GetValue(RadiusLineProperty);
            set => SetValue(RadiusLineProperty, value);
        }

        public static readonly BindableProperty CornerRadiusLineProperty = BindableProperty.Create(
            propertyName: nameof(CornerRadiusLine),
            returnType: typeof(int),
            declaringType: typeof(CustomSlider),
            defaultValue: 8,
            defaultBindingMode: BindingMode.OneWay);

        public int CornerRadiusLine
        {
            get => (int)GetValue(CornerRadiusLineProperty);
            set => SetValue(CornerRadiusLineProperty, value);
        }

        #endregion

        #region -- Private helpers --

        private void AppendToMapping()
        {
            SliderHandler.Mapper.AppendToMapping(nameof(Height), UpdateSlider);
            SliderHandler.Mapper.AppendToMapping(nameof(Width), UpdateSlider);
            SliderHandler.Mapper.AppendToMapping(nameof(Minimum), UpdateSlider);
            SliderHandler.Mapper.AppendToMapping(nameof(Maximum), UpdateSlider);
            SliderHandler.Mapper.AppendToMapping(nameof(Value), UpdateSlider);
            SliderHandler.Mapper.AppendToMapping(nameof(ThumbImageSource), UpdateSlider);
            SliderHandler.Mapper.AppendToMapping(nameof(MinimumTrackColor), UpdateSlider);
            SliderHandler.Mapper.AppendToMapping(nameof(MaximumTrackColor), UpdateSlider);
        }

        private void UpdateSlider(IViewHandler handler, IView view)
        {
            if (handler.PlatformView is Android.Widget.SeekBar progressBar && view is CustomSlider slider && progressBar.Height > 0 && progressBar.Width > 0)
            {
                progressBar.Post(() =>
                {
                    var density = Platform.AppContext.Resources.DisplayMetrics.Density;

                    int radiusLine = (int)(slider.RadiusLine * density);
                    int cornerRadiusLine = (int)(slider.CornerRadiusLine * density);

                    var widthTrack = progressBar.ProgressDrawable.Bounds.Width();
                    var padding = (progressBar.Width - widthTrack) / 2;

                    var percentValue = (slider.Value - slider.Minimum) / (slider.Maximum - slider.Minimum);

                    var centerY = progressBar.Height / 2;
                    var positionX = widthTrack * percentValue;

                    var bitmap = Bitmap.CreateBitmap(progressBar.Width, progressBar.Height, Bitmap.Config.Argb8888);

                    var canvas = new Canvas(bitmap);

                    var linePaint = new Paint();

                    linePaint.Color = slider.MaximumTrackColor.ToAndroid();

                    canvas.DrawRoundRect(padding, centerY - radiusLine, padding + widthTrack, centerY + radiusLine, cornerRadiusLine, cornerRadiusLine, linePaint);

                    linePaint.Color = slider.MinimumTrackColor.ToAndroid();

                    canvas.DrawRoundRect(padding, centerY - radiusLine, (int)positionX + padding, centerY + radiusLine, cornerRadiusLine, cornerRadiusLine, linePaint);

                    var drawable = new BitmapDrawable(progressBar.Resources, bitmap);

                    progressBar.Background = drawable;
                });
            }
        }

        #endregion
    }
}

