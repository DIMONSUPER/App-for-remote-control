using SmartMirror.Enums;
using SmartMirror.Interfaces;
using SmartMirror.Resources.DataTemplates;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class CategoryElementTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<ECategoryType, DataTemplate> _dataTemplatesDictionary = new()
        {
            { ECategoryType.Accessories, new SimpleAccessoryTemplate() },
            { ECategoryType.Scenarios, new SimpleScenarioTemplate() },
            { ECategoryType.Cameras, new SimpleCameraTemplate() },
            { ECategoryType.Providers, new ProvidersTemplate() },
        };

        #region -- Overrides --

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var key = ECategoryType.Scenarios;

            if (item is ICategoryElementModel category)
            {
                key = category.Type;
            }

            return _dataTemplatesDictionary[key];
        }

        #endregion
    }
}
