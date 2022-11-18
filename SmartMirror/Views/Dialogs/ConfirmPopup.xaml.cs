using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using SmartMirror.ViewModels.Dialogs;
using SmartMirror.Helpers;

namespace SmartMirror.Views.Dialogs;

public partial class ConfirmPopup : Popup
{
	public ConfirmPopup(ConfirmPopupViewModel confirmPopupViewModel)
	{
		InitializeComponent();

		confirmPopupViewModel.CloseCommand = SingleExecutionCommand.FromFunc(OnCloseCommandAsync);

        BindingContext = confirmPopupViewModel;
	}

    #region -- Private helpers --

	private Task OnCloseCommandAsync(object parameters)
	{
		Close(parameters);

		return Task.CompletedTask;
	}

    #endregion
}

