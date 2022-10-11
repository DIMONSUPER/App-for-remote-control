using System;
using SmartMirror.Models;
using Device = SmartMirror.Models.Device;

namespace SmartMirror.Services.Mock
{
    public interface ISmartHomeMockService
    {
        IEnumerable<Room> GetRooms();
        IEnumerable<NotificationModel> GetNotifications();
        IEnumerable<ScenarioModel> GetScenarios();
        IEnumerable<CameraModel> GetCameras();
        IEnumerable<Device> GetDevices();
    }
}

