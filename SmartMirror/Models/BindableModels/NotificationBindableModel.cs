using System.ComponentModel;
using SmartMirror.Enums;
using SmartMirror.Interfaces;
using SmartMirror.Resources;
using SmartMirror.Resources.Strings;

namespace SmartMirror.Models.BindableModels
{
    public class NotificationBindableModel : BindableBase
    {
        #region -- Public helpers --

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

        private bool _isReceiveNotifications;
        public bool IsReceiveNotifications
        {
            get => _isReceiveNotifications;
            set => SetProperty(ref _isReceiveNotifications, value);
        }

        private bool _isEmergencyNotification;
        public bool IsEmergencyNotification
        {
            get => _isEmergencyNotification;
            set => SetProperty(ref _isEmergencyNotification, value);
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

        private bool _isRoomNameVisible;
        public bool IsRoomNameVisible
        {
            get => _isRoomNameVisible;
            set => SetProperty(ref _isRoomNameVisible, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(Status) or nameof(Device))
            {
                StatusFormatted = GetStatusFormatted();
            }
        }

        #endregion

        #region -- Private helpers --

        private string GetStatusFormatted()
        {
            string result = null;

            if (!string.IsNullOrWhiteSpace(Status) && Device is not null)
            {
                result = Device.IconSource switch
                {
                    IconsNames.pic_humidity => $"{Strings.ChangedTo} {double.Parse(Status) / 100} %",
                    IconsNames.pic_pressure => $"{Math.Round(double.Parse(Status) / 1000, 2)} kPa",
                    IconsNames.pic_temperature => $"{Strings.ChangedTo} " + (Device.UnitMeasure == EUnitMeasure.Celsius ? $"{double.Parse(Status) / 100} ℃" : string.Format("{0:F2}", double.Parse(Status) / 100 * 1.8 + 32) + " ℉"),
                    IconsNames.pic_wall_switch_double_left => Status is "1" ? Strings.TurnedOn : Strings.TurnedOff,
                    IconsNames.pic_wall_switch_double_right => Status is "1" ? Strings.TurnedOn : Strings.TurnedOff,
                    IconsNames.pic_wall_switch_single => Status is "1" ? Strings.TurnedOn : Strings.TurnedOff,
                    IconsNames.pic_wall_switch_three_center => Status is "1" ? Strings.TurnedOn : Strings.TurnedOff,
                    IconsNames.pic_wall_switch_three_left => Status is "1" ? Strings.TurnedOn : Strings.TurnedOff,
                    IconsNames.pic_wall_switch_three_right => Status is "1" ? Strings.TurnedOn : Strings.TurnedOff,
                    IconsNames.pic_motion => Status is "1" ? Strings.TurnedOn : Strings.TurnedOff,
                    IconsNames.pic_dimmer => $"{Strings.ChangedTo} {Status} lux",
                    _ => Status,
                };
            }

            return result;
        }

        #endregion
    }
}
