namespace SmartMirror.Views.Dialogs;

public partial class BaseDialogView : Grid
{
    public BaseDialogView()
    {
        InitializeComponent();
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
}
