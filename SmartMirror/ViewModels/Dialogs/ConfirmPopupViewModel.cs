using System;
using System.Windows.Input;
using SmartMirror.Helpers;
using SmartMirror.Services.Blur;

namespace SmartMirror.ViewModels.Dialogs
{
    public class ConfirmPopupViewModel : BasePopupViewModel
    {
        public ConfirmPopupViewModel(
            IBlurService blurService)
            : base(blurService)
        {
        }

        #region -- Public properties --

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get => _closeCommand;
            set => SetProperty(ref _closeCommand, value);
        }

        private ICommand _confirmCommand;
        public ICommand ConfirmCommand => _confirmCommand ??= SingleExecutionCommand.FromFunc<bool>(OnConfirmCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogsParameterKeys.TITLE, out string title))
            {
                Title = title;
            }

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.DESCRIPTION, out string description))
            {
                Description = description;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnConfirmCommandAsync(bool confirm)
        {
            var parameters = new DialogParameters()
            {
                { Constants.DialogsParameterKeys.RESULT, confirm },
            };

            if (CloseCommand is not null && CloseCommand.CanExecute(parameters))
            {
                CloseCommand.Execute(parameters);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}

