using SmartMirror.Enums;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Mock
{
    public class SmartHomeMockService : ISmartHomeMockService
    {
        public IEnumerable<CameraBindableModel> GetCameras()
        {
            List<CameraBindableModel> cameras= new()
            {
                new() 
                { 
                    Id = 1,
                    Name = "Front Door 1",
                    IsShown = true,
                    IsConnected = true,
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://videos-3.earthcam.com/fecnetwork/15659.flv/chunklist_w999153032.m3u8",
                },
                new() 
                {
                    Id = 2,
                    Name = "Front Door 2",
                    IsShown = true,
                    IsConnected = true, 
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://videos-3.earthcam.com/fecnetwork/15659.flv/chunklist_w999153032.m3u8",
                },
                new() 
                { 
                    Id = 3,
                    Name = "Back Door 1",
                    IsShown = true,
                    IsConnected = false,
                    CreateTime = DateTime.Now,
                },
                new() 
                {
                    Id = 4,
                    Name = "Back Door 2",
                    IsShown = true,
                    IsConnected = true, 
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://videos-3.earthcam.com/fecnetwork/17568.flv/chunklist_w1596475220.m3u8",
                },
                new() 
                {
                    Id = 5,
                    Name = "Garage",
                    CreateTime = DateTime.Now,
                    IsShown = true,
                    IsConnected = false,
                },
            };

            return cameras;
        }

        public IEnumerable<NotificationGroupItemBindableModel> GetNotifications()
        {
            List<NotificationGroupItemBindableModel> notifications = new()
            {
                new()
                {
                    IsShown = true,
                    Status = "Opened",
                    LastActivityTime = DateTime.Now,
                },
                new()
                {
                    IsShown = true,
                    Status = "No movement",
                    LastActivityTime = DateTime.Now.AddMinutes(-15),
                },
                new()
                {
                    IsShown = true,
                    Status = "Unknown",
                    LastActivityTime = DateTime.Now.AddHours(-3),
                },
                new()
                {
                    IsShown = true,
                    Status = "Closed",
                    LastActivityTime = DateTime.Now.AddDays(-1),
                },
                new()
                {
                    IsShown = true,
                    Status = "Opened",
                    LastActivityTime = DateTime.Now.AddDays(-2),
                },
            };

            return notifications;
        }

        public IEnumerable<DeviceModel> GetDevices()
        {
            List<DeviceModel> devices = new()
            {
                new() { Name = "Garage Door", IconSource = "garage_door", Status = EDeviceStatus.On, DeviceType = EDeviceType.Locker },
                new() { Name = "Front Door",  IconSource = "front_door", Status = EDeviceStatus.On, DeviceType = EDeviceType.Locker },
                new FanDevice { Name = "Fan", IconSource = "fan", RoomName = "Living Room" , Status = EDeviceStatus.On, AdditionalInfo = "68%", DeviceType = EDeviceType.Switcher }
            };

            return devices;
        }
    }
}

