using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;
using SmartMirror.Services.Devices;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class AccessorySettingsDialogViewModel : BaseDialogViewModel
    {
        private readonly IDevicesService _devicesService;
        private bool _isInitializing = true;

        public AccessorySettingsDialogViewModel(
            IDevicesService devicesService,
            IBlurService blurService,
            IKeyboardService keyboardService)
            : base(blurService, keyboardService)
        {
            _devicesService = devicesService;
        }

        #region -- Public properties --

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _imageSource;
        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private bool _isShownInRooms;
        public bool IsShownInRooms
        {
            get => _isShownInRooms;
            set => SetProperty(ref _isShownInRooms, value);
        }

        private bool _isReceiveNotifications;
        public bool IsReceiveNotifications
        {
            get => _isReceiveNotifications;
            set => SetProperty(ref _isReceiveNotifications, value);
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        private bool _isEmergencyNotification;
        public bool IsEmergencyNotification
        {
            get => _isEmergencyNotification;
            set => SetProperty(ref _isEmergencyNotification, value);
        }

        private DeviceBindableModel _accessory;
        public DeviceBindableModel Accessory
        {
            get => _accessory;
            set => SetProperty(ref _accessory, value);
        }

        private ICommand _changeUnitMeasureCommand;
        public ICommand ChangeUnitMeasureCommand => _changeUnitMeasureCommand ??= SingleExecutionCommand.FromFunc<EUnitMeasure>(OnChangeUnitMeasureCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.ACCESSORY, out ImageAndTitleBindableModel accessory))
            {
                Title = accessory.Name;
                ImageSource = accessory.ImageSource;

                Accessory = accessory.Model as DeviceBindableModel;

                IsFavorite = Accessory.IsFavorite;
                IsShownInRooms = Accessory.IsShownInRooms;
                IsReceiveNotifications = Accessory.IsReceiveNotifications;
                IsEmergencyNotification = Accessory.IsEmergencyNotification;
            }

            _isInitializing = false;
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (!_isInitializing
                && args.PropertyName is nameof(IsFavorite)
                or nameof(IsShownInRooms)
                or nameof(IsReceiveNotifications)
                or nameof(IsEmergencyNotification))
            {
                Accessory.IsFavorite = _isFavorite;
                Accessory.IsShownInRooms = _isShownInRooms;
                Accessory.IsReceiveNotifications = _isReceiveNotifications;
                Accessory.IsEmergencyNotification = _isEmergencyNotification;

                await _devicesService.UpdateDeviceAsync(Accessory);
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnChangeUnitMeasureCommandAsync(EUnitMeasure unitMeasure)
        {
            Accessory.UnitMeasure = unitMeasure;

            return _devicesService.UpdateDeviceAsync(Accessory);
        }

        #endregion
    }
}
