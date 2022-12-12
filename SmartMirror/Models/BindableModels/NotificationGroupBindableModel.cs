namespace SmartMirror.Models.BindableModels
{
    public class NotificationGroupBindableModel : List<NotificationGroupItemBindableModel>
    {
        public string GroupName { get; private set; } = string.Empty;

        public NotificationGroupBindableModel(string groupName, List<NotificationGroupItemBindableModel> items) : base(items)
        {
            GroupName = groupName;
        }
    }
}