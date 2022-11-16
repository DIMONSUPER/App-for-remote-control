using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartMirror.Controls
{
    public class CustomScrollView : ScrollView
    {
        #region -- Public properties --

        public static readonly BindableProperty BindingForInvalidateMeasureProperty = BindableProperty.Create(
           propertyName: nameof(BindingForInvalidateMeasure),
           returnType: typeof(object),
           declaringType: typeof(CustomScrollView));

        public object BindingForInvalidateMeasure
        {
            get => GetValue(BindingForInvalidateMeasureProperty);
            set => SetValue(BindingForInvalidateMeasureProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            (this as IView).InvalidateMeasure();
        }

        #endregion
    }
}
