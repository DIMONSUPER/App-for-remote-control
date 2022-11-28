
using SmartMirror.Helpers;
using SmartMirror.Models.DTO;

namespace SmartMirror.Services.Automation;

public interface IAutomationService
{
    event EventHandler AllAutomationsChanged;

    Task<IEnumerable<AutomationDTO>> GetAllAutomationsAsync();

    Task<AOResult> DownloadAllAutomationsAsync();
}

