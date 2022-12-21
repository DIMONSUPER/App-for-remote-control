
using SmartMirror.Helpers;
using SmartMirror.Models.DTO;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Automation;

public interface IAutomationService
{
    event EventHandler AllAutomationsChanged;

    Task<IEnumerable<AutomationBindableModel>> GetAllAutomationsAsync();

    Task<AOResult> DownloadAllAutomationsAsync();

    Task<AOResult> UpdateAutomationAsync(AutomationBindableModel bindableAutomation);

    Task<AOResult<DataAqaraResponse<LinkageAqaraModel>>> GetLinkagesAsync(string positionId = null, int pageNum = 1, int pageSize = 100);

    Task<AOResult<LinkageDetailAqaraModel>> GetLinkageDetailAsync(string linkageId);

    Task<AOResult<BaseAqaraResponse>> ChangeLinkageStateAsync(string linkageId, bool isEnabled);
}

