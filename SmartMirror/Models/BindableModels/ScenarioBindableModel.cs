using SmartMirror.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.Models.BindableModels
{
    public class ScenarioBindableModel : BindableBase, INotifiable
    {
        #region -- Public properties --

        private string _sceneId;
        public string SceneId
        {
            get => _sceneId;
            set => SetProperty(ref _sceneId, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        private bool _isShownInScenarios = true;
        public bool IsShownInScenarios
        {
            get => _isShownInScenarios;
            set => SetProperty(ref _isShownInScenarios, value);
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        private bool _isEmergencyNotification = true;
        public bool IsEmergencyNotification
        {
            get => _isEmergencyNotification;
            set => SetProperty(ref _isEmergencyNotification, value);
        }

        private DateTime _activationTime;
        public DateTime ActivationTime
        {
            get => _activationTime;
            set => SetProperty(ref _activationTime, value);
        }

        private ObservableCollection<ScenarioActionBindableModel> _actions;
        public ObservableCollection<ScenarioActionBindableModel> Actions
        {
            get => _actions;
            set => SetProperty(ref _actions, value);
        }

        private bool _isUpdating;
        public bool IsUpdating
        {
            get => _isUpdating;
            set => SetProperty(ref _isUpdating, value);
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

        #endregion

        #region -- INotifiable implementation --

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

        private bool _isReceiveNotifications = true;
        public bool IsReceiveNotifications
        {
            get => _isReceiveNotifications;
            set => SetProperty(ref _isReceiveNotifications, value);
        }

        #endregion
    }
}
