using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Blur;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class ScenarioSettingsDialogViewModel : BaseDialogViewModel
    {
        public ScenarioSettingsDialogViewModel(IBlurService blurService)
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

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= SingleExecutionCommand.FromFunc(OnCloseCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogsParameterKeys.SCENARIO, out ImageAndTitleBindableModel scenario))
            {
                Title = scenario.Name;
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
