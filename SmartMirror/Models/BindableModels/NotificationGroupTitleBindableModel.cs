namespace SmartMirror.Models.BindableModels
{
    public class NotificationGroupTitleBindableModel : BindableBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
