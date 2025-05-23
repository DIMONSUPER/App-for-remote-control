using SmartMirror.Helpers;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;
using SmartMirror.Resources.Strings;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class ConfirmDialogViewModel : BaseDialogViewModel
    {
        private Action _confirmAction;

        public ConfirmDialogViewModel(
            IBlurService blurService,
            IKeyboardService keyboardService)
            : base(blurService, keyboardService)
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

        private string _cancelText = Strings.Close;
        public string CancelText
        {
            get => _cancelText;
            set => SetProperty(ref _cancelText, value);
        }

        private string _confirmText = Strings.Confirm;
        public string ConfirmText
        {
            get => _confirmText;
            set => SetProperty(ref _confirmText, value);
        }

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.TITLE, out string title))
            {
                Title = title;
            }

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.DESCRIPTION, out string description))
            {
                Description = description;
            }

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.CONFIRM_ACTION, out Action action))
            {
                _confirmAction = action;
            }

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.CONFIRM_TEXT, out string confirmText))
            {
                ConfirmText = confirmText;
            }

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.CANCEL_TEXT, out string cancelText))
            {
                CancelText = cancelText;
            }
        }

        public override Task OnCloseCommandAsync(object parameter)
        {
            var isConfrimed = parameter is bool confirm ? confirm : false;

            if (isConfrimed)
            {
                _confirmAction?.Invoke();
            }

            RequestClose.Invoke(new DialogParameters()
            {
                { Constants.DialogsParameterKeys.RESULT, isConfrimed },
            });

            return Task.CompletedTask;
        }

        #endregion
    }
}

