using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.DataTemplates;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class CategoryElementTemplate : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> _dataTemplatesDictionary = new()
        {
            { typeof(ImageAndTitleBindableModel), new ImageAndTitleTemplate() },
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
