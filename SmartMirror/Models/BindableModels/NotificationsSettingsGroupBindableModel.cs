using SmartMirror.Enums;
using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
    public class NotificationSettingsGroupBindableModel : BindableBase, ICategoryElementModel
    {
        #region -- Public properties --
        
        private string _groupName;
        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        private List<ImageAndTitleBindableModel> _notificationSettings;
        public List<ImageAndTitleBindableModel> NotificationSettings
        {
            get => _notificationSettings;
            set => SetProperty(ref _notificationSettings, value);
        }

        #endregion

        #region -- ICategoryElementModel implementation --

        private string _imageSource;
        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private ECategoryType _type;
        public ECategoryType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        #endregion
    }
}
