using System.Windows.Input;

namespace SmartMirror.Models.BindableModels
{
    public class NotificationSourceBindableModel : BindableBase
    {
        #region -- Public properties --

        private string _id;
        public string Id
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

        private int _notificationsCount;
        public int NotificationsCount
        {
            get => _notificationsCount;
            set => SetProperty(ref _notificationsCount, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private ICommand _selectCommand;
        public ICommand SelectCommand
        {
            get => _selectCommand;
            set => SetProperty(ref _selectCommand, value);
        }

        #endregion
    }
}