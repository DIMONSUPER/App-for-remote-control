using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
    public class NotificationGroupBindableModel : List<NotificationGroupItemBindableModel>, IGroupableCollection
    {
        public NotificationGroupBindableModel(string groupName, List<NotificationGroupItemBindableModel> items) : base(items)
        {
            GroupName = groupName;
            ItemsCount = items.Count;
        }

        #region -- IGroupableCollection implementation --

        public string GroupName { get; set; } = string.Empty;

        public int ItemsCount { get; set; }

        #endregion
    }
}