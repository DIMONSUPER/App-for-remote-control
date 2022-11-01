using System;
using SmartMirror.Helpers;
using System.Windows.Input;
using SmartMirror.Platforms.Android.Services;
using SmartMirror.Services.Blur;
using Android.Telephony.Data;

namespace SmartMirror.ViewModels.Dialogs
{
    public class ConfirmDialogViewModel : BaseDialogViewModel
    {
        public ConfirmDialogViewModel(
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
        public ICommand CloseCommand => _closeCommand ??= SingleExecutionCommand.FromFunc<string>(OnCloseCommandAsync);

        #endregion

        #region Overrides --

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
        }

        #endregion

        #region -- Private helpers --

        private Task OnCloseCommandAsync(string result)
        {
            var parameters = new DialogParameters();

            switch (result.ToLower())
            {
                case "true": { parameters.Add(Constants.DialogsParameterKeys.RESULT, true); break; }
                case "false": { parameters.Add(Constants.DialogsParameterKeys.RESULT, false); break; }
                default:
                    break;
            }

            RequestClose.Invoke(parameters);

            return Task.CompletedTask;
        }

        #endregion

    }
}

