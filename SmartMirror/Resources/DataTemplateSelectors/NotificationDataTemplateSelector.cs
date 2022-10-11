using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.DataTemplates;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class NotificationDataTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<string, DataTemplate> _dataTemplatesDictionary = new()
        {
            { typeof(NotificationGroupItemBindableModel).ToString(), new NotificationTemplate() },
            { typeof(NotificationGroupTitleBindableModel).ToString(), new NotificationHeaderTemplate() },
        };

        #region -- Overrides --

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var key = item.GetType().ToString();

            return _dataTemplatesDictionary[key];
        }

        #endregion
    }
}
