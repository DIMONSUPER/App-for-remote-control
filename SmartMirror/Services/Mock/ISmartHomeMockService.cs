using SmartMirror.Models;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Mock
{
    public interface ISmartHomeMockService
    {
        IEnumerable<RoomModel> GetRooms();
        IEnumerable<NotificationGroupItemBindableModel> GetNotifications();
        IEnumerable<ScenarioModel> GetScenarios();
        IEnumerable<CameraBindableModel> GetCameras();
        IEnumerable<DeviceModel> GetDevices();
    }
}

