using System;
using SmartMirror.Models;
using DeviceModel = SmartMirror.Models.DeviceModel;

namespace SmartMirror.Services.Mock
{
    public interface ISmartHomeMockService
    {
        IEnumerable<RoomModel> GetRooms();
        IEnumerable<NotificationModel> GetNotifications();
        IEnumerable<ScenarioModel> GetScenarios();
        IEnumerable<CameraModel> GetCameras();
        IEnumerable<DeviceModel> GetDevices();
    }
}

