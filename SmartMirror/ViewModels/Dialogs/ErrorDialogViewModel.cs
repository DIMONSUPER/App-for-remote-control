using SmartMirror.Helpers;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class ErrorDialogViewModel : BindableBase, IDialogAware
    {
        public ErrorDialogViewModel()
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
        public ICommand CloseCommand => _closeCommand ??= SingleExecutionCommand.FromFunc(OnCloseCommandAsync);

        #endregion

        #region -- IDialogAware implementation --

        public DialogCloseEvent RequestClose { get; set; }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
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

        private Task OnCloseCommandAsync()
        {
            RequestClose.Invoke();

            return Task.CompletedTask;
        }

        #endregion
    }
}
