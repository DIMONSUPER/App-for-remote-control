using System.Runtime.CompilerServices;

namespace SmartMirror.Controls
{
    public class CustomScrollView : ScrollView
    {
        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            (this as IView)?.InvalidateMeasure();
        }

        #endregion
    }
}
