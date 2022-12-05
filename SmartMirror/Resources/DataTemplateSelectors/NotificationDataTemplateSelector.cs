using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.DataTemplates;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class NotificationDataTemplateSelector : DataTemplateSelector
    {
        #region -- Public properties --

        public DataTemplate NotificationDataTemplate { get; set; }

        public DataTemplate NotificationHeaderDataTemplate { get; set; }

        #endregion

        #region -- Overrides --

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            DataTemplate result = null;

            result = item.GetType().Name switch
            {
                nameof(NotificationGroupTitleBindableModel) => NotificationHeaderDataTemplate,
                nameof(NotificationGroupItemBindableModel) => NotificationDataTemplate,
            };

            return result;
        }

        #endregion
    }
}
