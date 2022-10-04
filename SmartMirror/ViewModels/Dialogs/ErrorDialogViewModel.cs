namespace SmartMirror.ViewModels.Dialogs
{
    public class ErrorDialogViewModel : BindableBase, IDialogAware
    {
        public ErrorDialogViewModel()
        {
            CloseCommand = new DelegateCommand(() => RequestClose.Invoke());
        }

        #region -- Public properties --

        private string _title = "Title";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _description = "Description";
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public DelegateCommand CloseCommand { get; }

        public DialogCloseEvent RequestClose { get; set; }

        #endregion

        #region -- IDialogAware implementation --

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
    }
}
