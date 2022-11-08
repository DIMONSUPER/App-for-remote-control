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

        private DeviceBindableModel _device;
        public DeviceBindableModel Device
        {
            get => _device;
            set => SetProperty(ref _device, value);
        }

        private bool _isShown;
        public bool IsShown
        {
            get => _isShown;
            set => SetProperty(ref _isShown, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _statusFormatted;
        public string StatusFormatted
        {
            get => _statusFormatted;
            set => SetProperty(ref _statusFormatted, value);
        }

        private DateTime _lastActivityTime;
        public DateTime LastActivityTime
        {
            get => _lastActivityTime;
            set => SetProperty(ref _lastActivityTime, value);
        }
    }
}
