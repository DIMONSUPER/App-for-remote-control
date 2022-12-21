using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
    public class NotificationGroupBindableModel : List<NotificationBindableModel>, IGroupableCollection
    {
        public NotificationGroupBindableModel(string groupName, IEnumerable<NotificationBindableModel> items) : base(items)
        {
            GroupName = groupName;
            ItemsCount = items.Count();
        }

        #region -- IGroupableCollection implementation --

        public string GroupName { get; set; } = string.Empty;

        public int ItemsCount { get; set; }

        #endregion
    }
}