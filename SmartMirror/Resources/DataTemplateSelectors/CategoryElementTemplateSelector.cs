using SmartMirror.Enums;
using SmartMirror.Interfaces;
using SmartMirror.Resources.DataTemplates;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class CategoryElementTemplateSelector : DataTemplateSelector
    {
        #region -- Public properties --

        public DataTemplate AccessoriesDataTemplate { get; set; }

        public DataTemplate ScenariosDataTemplate { get; set; }

        public DataTemplate CamerasDataTemplate { get; set; }

        public DataTemplate ProvidersDataTemplate { get; set; }

        public DataTemplate NotificationsDataTemplate { get; set; }

        #endregion

        #region -- Overrides --

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            DataTemplate result = null;

            if (item is ICategoryElementModel category)
            {
                result = category.Type switch
                {
                    ECategoryType.Accessories => AccessoriesDataTemplate,
                    ECategoryType.Scenarios => ScenariosDataTemplate,
                    ECategoryType.Cameras => CamerasDataTemplate,
                    ECategoryType.Providers => ProvidersDataTemplate,
                    ECategoryType.Notifications => NotificationsDataTemplate,
                };
            }

            return result;
        }

        #endregion
    }
}
