using Android.Graphics;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using System.ComponentModel;
using static Android.Graphics.Drawables.ShapeDrawable;
using AView = Android.Views.View;
using Color = Microsoft.Maui.Graphics.Color;
using ShapeDrawable = Android.Graphics.Drawables.ShapeDrawable;

namespace SmartMirror.Effects
{
    public class LinearGradientEffect : RoutingEffect
    {
        internal const string StartColorPropertyName = "StartColor";
        internal const string EndColorPropertyName = "EndColor";

        public static readonly BindableProperty StartColorProperty = BindableProperty.CreateAttached(
            StartColorPropertyName,
            typeof(Color),
            typeof(LinearGradientEffect),
            Colors.White,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty EndColorProperty = BindableProperty.CreateAttached(
            EndColorPropertyName,
            typeof(Color),
            typeof(LinearGradientEffect),
            Colors.Black,
            propertyChanged: TryGenerateEffect);

        public static Color GetStartColor(BindableObject bindable)
            => (Color)bindable.GetValue(StartColorProperty);

        public static void SetStartColor(BindableObject bindable, Color value)
            => bindable.SetValue(StartColorProperty, value);

        public static Color GetEndColor(BindableObject bindable)
            => (Color)bindable.GetValue(EndColorProperty);

        public static void SetEndColor(BindableObject bindable, Color value)
            => bindable.SetValue(EndColorProperty, value);

        static void TryGenerateEffect(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is VisualElement view)
            {
                var gradientEffects = view.Effects.OfType<LinearGradientEffect>();

                if (GetStartColor(view) == Colors.White && GetEndColor(view) == Colors.Black)
                {
                    foreach (var effect in gradientEffects.ToArray())
                    {
                        view.Effects.Remove(effect);
                    }

                    return;
                }
                else if (!gradientEffects.Any())
                {
                    view.Effects.Add(new LinearGradientEffect());
                }
            }
        }
    }

    public class ShadowPlatformEffect : PlatformEffect
    {
        AView View => Control ?? Container;

        protected override void OnAttached()
            => Update();

        protected override void OnDetached()
        {
            if (View == null)
            {
                return;
            }

            View.Elevation = 0;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (View == null)
            {
                return;
            }

            switch (args.PropertyName)
            {
                case LinearGradientEffect.StartColorPropertyName:
                case LinearGradientEffect.EndColorPropertyName:
                case nameof(VisualElement.Width):
                case nameof(VisualElement.Height):
                case nameof(VisualElement.BackgroundColor):
                case nameof(IBorderElement.CornerRadius):
                    View.Invalidate();
                    Update();
                    break;
            }
        }

        void Update()
        {
            if (View != null)
            {
                //var color1 = LinearGradientBackgroundEffect.GetStartColor(Element).ToInt();
                //var color2 = LinearGradientBackgroundEffect.GetEndColor(Element).ToInt();

                //GradientDrawable gd = new GradientDrawable(GradientDrawable.Orientation.TlBr, new int[] { color1, color2});

                //View.SetBackground(gd);

                /////////////////////////////////////////

                var startColor = LinearGradientEffect.GetStartColor(Element).ToAndroid();
                var endColor = LinearGradientEffect.GetEndColor(Element).ToAndroid();

                var gradientFactory = new LinearGradientShaderFactory()
                {
                    StartColor = startColor,
                    EndColor = endColor,
                };

                ShapeDrawable shapeDrawable = new ShapeDrawable(new RectShape());
                shapeDrawable.SetDither(true);
                shapeDrawable.SetShaderFactory(gradientFactory);

                View.SetBackground(shapeDrawable);
                View.Background.SetDither(true);

                /////////////////////////////////////////
            }
        }

        class LinearGradientShaderFactory : ShaderFactory
        {
            public Android.Graphics.Color StartColor { get; set; }

            public Android.Graphics.Color EndColor { get; set; }

            public override Shader Resize(int width, int height)
            {
                return new LinearGradient(0, 0, width, height, StartColor, EndColor, Shader.TileMode.Mirror);
            }
        }
    }
}
