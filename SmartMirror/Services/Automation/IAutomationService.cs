
using SmartMirror.Helpers;
using SmartMirror.Models.DTO;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Automation;

public interface IAutomationService
{
    event EventHandler AllAutomationsChanged;

    Task<IEnumerable<AutomationBindableModel>> GetAllAutomationsAsync();

    Task<AOResult> DownloadAllAutomationsAsync();

    Task<AOResult> UpdateAutomationAsync(AutomationBindableModel bindableAutomation);
}

