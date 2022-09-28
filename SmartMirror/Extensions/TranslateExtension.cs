using System;
using SmartMirror.Helpers;
using SmartMirror.Resources.Strings;

namespace SmartMirror.Extensions
{
    [ContentProperty(nameof(Name))]
    public class TranslateExtension : IMarkupExtension
    {
        public string? Name { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Name}]",
                Source = LocalizationResourceManager.Instance
            };
        }

        #region -- IMarkupExtension implementation --

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }

        #endregion
    }
}

