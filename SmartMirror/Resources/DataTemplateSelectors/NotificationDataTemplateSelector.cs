using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.DataTemplates;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class NotificationDataTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> _dataTemplatesDictionary = new()
        {
            { typeof(NotificationGroupItemBindableModel), new NotificationTemplate() },
            { typeof(NotificationGroupTitleBindableModel), new NotificationHeaderTemplate() },
        };

        #region -- Overrides --

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var key = item.GetType();

            return _dataTemplatesDictionary[key];
        }

        #endregion
    }
}
