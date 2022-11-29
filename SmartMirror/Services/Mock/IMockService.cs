using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Mock
{
    public interface IMockService
    {
        IEnumerable<AutomationBindableModel> GetAutomation();
    }
}
