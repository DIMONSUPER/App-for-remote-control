using System.Runtime.CompilerServices;

namespace SmartMirror.Views.Dialogs;

public partial class BaseDialogView : Grid
{
    public BaseDialogView()
    {
        InitializeComponent();

        Unloaded += OnUnloaded;
    }

    #region -- Overrides --

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is ViewModels.Dialogs.BaseDialogViewModel vm)
        {
            this.SetBinding(HeightProperty, nameof(vm.ViewHeight), BindingMode.OneWayToSource);
        }
    }

    #endregion

    #region -- Private helpers --

    private void OnUnloaded(object sender, EventArgs e)
    {
        //this.Unfocus();
    }

    #endregion
}
