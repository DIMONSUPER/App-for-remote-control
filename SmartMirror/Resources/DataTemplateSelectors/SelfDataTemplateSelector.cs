using System;
namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class SelfDataTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<string, DataTemplate> _dataTemplatesDictionary = new();

        #region -- Overrides --

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var key = item.GetType().ToString();

            if (!_dataTemplatesDictionary.ContainsKey(key))
            {
                _dataTemplatesDictionary[key] = new DataTemplate(item.GetType());
            }

            return _dataTemplatesDictionary[key];
        }

        #endregion
    }
}

