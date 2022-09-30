using System;
using SmartMirror.Models;
using Device = SmartMirror.Models.Device;

namespace SmartMirror.Services.Mock
{
    public interface IMockService
    {
        List<Room> GetRooms();
        List<Notification> GetNotifications();
        List<Scenario> GetScenarios();
        List<Camera> GetCameras();
        List<Device> GetDevices();
    }
}

