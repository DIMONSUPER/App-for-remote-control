using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
    public class NotificationGroupTitleBindableModel : BindableBase, INotificationGroupItemModel
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
