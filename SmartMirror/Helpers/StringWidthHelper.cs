using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Util;

namespace SmartMirror.Helpers
{
	public static class StringWidthHelper
	{
		private static readonly Dictionary<string, string> _fontPaths = new Dictionary<string, string>
		{
			{ "InterMedium", "Inter-Medium-500.ttf" },
			{ "InterSemiBold", "Inter-SemiBold-600.ttf" },
			{ "InterBold", "Inter-Bold-700.ttf" },
		};

		#region -- Public helpers --

		public static float CalculateStringWidth(string text, float fontSize, string fontFamily)
		{
			float stringWidth = 0f;

			if (text is not null && fontSize > 0 && _fontPaths.TryGetValue(fontFamily, out string fontPath))
			{
				var textPaint = new TextPaint
				{
					AntiAlias = true,
                    TextSize = TypedValue.ApplyDimension(ComplexUnitType.Sp, fontSize, Platform.AppContext.Resources.DisplayMetrics),
				};

				textPaint.SetTypeface(Typeface.CreateFromAsset(Platform.AppContext.Assets, fontPath));

                stringWidth = textPaint.MeasureText(text) / Platform.AppContext.Resources.DisplayMetrics.Density;
            }

			return stringWidth;
		}

		#endregion
	}
}

