namespace SmartMirror.Controls
{
    public class BlurredImage : Image
    {
        public BlurredImage()
        {
        }

        #region -- Public properties --

        public static readonly BindableProperty RadiusProperty = BindableProperty.Create(
            propertyName: nameof(Radius),
            returnType: typeof(int),
            declaringType: typeof(BlurredImage),
            defaultBindingMode: BindingMode.OneWay);

        public int Radius
        {
            get => (int)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        #endregion
    }
}
