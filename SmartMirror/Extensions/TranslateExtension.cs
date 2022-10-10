using Microsoft.Extensions.Localization;
using SmartMirror.Resources.Strings;

namespace SmartMirror.Extensions;

[ContentProperty(nameof(Name))]
public class TranslateExtension : IMarkupExtension
{
    private readonly IStringLocalizer<Strings> _localizer;

    public TranslateExtension()
    {
        _localizer = MauiApplication.Current.Services.GetService<IStringLocalizer<Strings>>();
    }

    #region -- Public properties --

    public string Name { get; set; }

    #endregion

    #region -- IMarkupExtension implementation --

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    #endregion

    #region -- Public methods --

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return _localizer[Name];
    }

    #endregion
}

