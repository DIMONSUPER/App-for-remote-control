using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.DataTemplates;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class CategoryElementTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> _dataTemplatesDictionary = new()
        {
            { typeof(ImageAndTitleBindableModel), new ImageAndTitleTemplate() },
            { typeof(SettingsProvidersBindableModel), new ProvidersTemplate() },
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
