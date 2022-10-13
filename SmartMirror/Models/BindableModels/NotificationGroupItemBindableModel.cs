using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
    public class NotificationGroupItemBindableModel : BindableBase, INotificationGroupItemModel
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

        private string _type;
        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private DateTime _lastActivityTime;
        public DateTime LastActivityTime
        {
            get => _lastActivityTime;
            set => SetProperty(ref _lastActivityTime, value);
        }
    }
}
