using SmartMirror.Models;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Mock
{
    public interface ISmartHomeMockService
    {
        IEnumerable<NotificationGroupItemBindableModel> GetNotifications();
        IEnumerable<CameraBindableModel> GetCameras();
        IEnumerable<DeviceModel> GetDevices();
    }
}

