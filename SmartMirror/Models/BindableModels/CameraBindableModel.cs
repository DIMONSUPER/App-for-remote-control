using SmartMirror.Interfaces;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartMirror.Models.BindableModels
{
    public class CameraBindableModel : BindableBase, ITappable, INotifiable
    {
        #region -- Public properties --

        private string _ipAddress;
        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        private string _login;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private bool _isShown;
        public bool IsShown
        {
            get => _isShown;
            set => SetProperty(ref _isShown, value);
        }

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get => _createTime;
            set => SetProperty(ref _createTime, value);
        }

        private string _videoUrl;
        public string VideoUrl
        {
            get => _videoUrl;
            private set => SetProperty(ref _videoUrl, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isEmergencyNotification = true;
        public bool IsEmergencyNotification
        {
            get => _isEmergencyNotification;
            set => SetProperty(ref _isEmergencyNotification, value);
        }

        private bool _isMuted;
        public bool IsMuted
        {
            get => _isMuted;
            set => SetProperty(ref _isMuted, value);
        }

        private int _channel;
        public int Channel
        {
            get => _channel;
            set => SetProperty(ref _channel, value);
        }

        private string _sessionId;
        public string SessionId
        {
            get => _sessionId;
            set => SetProperty(ref _sessionId, value);
        }

        private int _requestId;
        public int RequestId
        {
            get => _requestId;
            set => SetProperty(ref _requestId, value);
        }

        private int _subType;
        public int SubType
        {
            get => _subType;
            set => SetProperty(ref _subType, value);
        }

        #endregion

        #region -- ITappable implementation --

        private ICommand _tapCommand;
        public ICommand TapCommand
        {
            get => _tapCommand;
            set => SetProperty(ref _tapCommand, value);
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

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(Login) or nameof(Password) or nameof(IpAddress) or nameof(SubType))
            {
                VideoUrl = $"rtsp://{Login}:{Password}@{IpAddress}:80/cam/realmonitor?channel=1&subtype={SubType}";
            }
        }

        #endregion
    }
}
