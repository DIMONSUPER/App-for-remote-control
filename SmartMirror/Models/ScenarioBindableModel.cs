using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.Models
{
    public class ScenarioBindableModel : BindableBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        private DateTime _activationTime;
        public DateTime ActivationTime
        {
            get => _activationTime;
            set => SetProperty(ref _activationTime, value);
        }

        private ObservableCollection<ScenarioAction> _scenarioActions;
        public ObservableCollection<ScenarioAction> ScenarioActions
        {
            get => _scenarioActions;
            set => SetProperty(ref _scenarioActions, value);
        }

        private ICommand _tappedCommand;
        public ICommand TappedCommand
        {
            get => _tappedCommand;
            set => SetProperty(ref _tappedCommand, value);
        }

        private ICommand _changeActiveStatusCommand;
        public ICommand ChangeActiveStatusCommand
        {
            get => _changeActiveStatusCommand;
            set => SetProperty(ref _changeActiveStatusCommand, value);
        }
    }
}
