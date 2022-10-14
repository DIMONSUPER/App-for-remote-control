using SmartMirror.Enums;
using SmartMirror.Models;

namespace SmartMirror.Services.Mock
{
    public class SmartHomeMockService : ISmartHomeMockService
    {
        public IEnumerable<CameraModel> GetCameras()
        {
            List<CameraModel> cameras= new()
            {
                new() 
                { 
                    Id = 1,
                    Name = "Front Door 1", 
                    IsConnected = true,
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://videos-3.earthcam.com/fecnetwork/hdtimes10.flv/chunklist_w53922196.m3u8",
                },
                new() 
                {
                    Id = 2,
                    Name = "Front Door 2",
                    IsConnected = true, 
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://videos-3.earthcam.com/fecnetwork/15659.flv/chunklist_w999153032.m3u8",
                },
                new() 
                { 
                    Id = 3,
                    Name = "Back Door 1",
                    IsConnected = false,
                    CreateTime = DateTime.Now,
                },
                new() 
                {
                    Id = 4,
                    Name = "Back Door 2",
                    IsConnected = true, 
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://videos-3.earthcam.com/fecnetwork/17568.flv/chunklist_w1596475220.m3u8",
                },
                new() 
                {
                    Id = 5,
                    Name = "Garage",
                    CreateTime = DateTime.Now,
                    IsConnected = false,
                },
            };

            return cameras;
        }

        public IEnumerable<NotificationModel> GetNotifications()
        {
            List<NotificationModel> notifications = new()
            {
                new()
                {
                    Name = "Garage Door",
                    Type = "Garage",
                    Status = "Opened",
                    LastActivityTime = DateTime.Now,
                },
                new()
                {
                    Name = "Upstairs Hallway Movement",
                    Type = "NoMovement",
                    Status = "No movement",
                    LastActivityTime = DateTime.Now.AddMinutes(-15),
                },
                new()
                {
                    Name = "Balcony Door",
                    Type = "Unknown",
                    Status = "Unknown",
                    LastActivityTime = DateTime.Now.AddHours(-3),
                },
                new()
                {
                    Name = "Balcony Door",
                    Type = "Door",
                    Status = "Closed",
                    LastActivityTime = DateTime.Now.AddDays(-1),
                },
                new()
                {
                    Name = "Garage Door",
                    Type = "Garage",
                    Status = "Opened",
                    LastActivityTime = DateTime.Now.AddDays(-2),
                },
            };

            return notifications;
        }

        public IEnumerable<RoomModel> GetRooms()
        {
            List<RoomModel> rooms = new()
            {
                new() { Id = "real2.1999738989430480896", Name = "Dining Room", Description = "0 Accessories"},
                new() { Id = "real2.2999738989430480896", Name = "Downstairs", Description = "0 Accessories"},
                new() { Id = "real2.3999738989430480896", Name = "Front Door", Description = "0 Accessories"},
                new() { Id = "real2.4999738989430480896", Name = "Garage", Description = "0 Accessories" },
            };

            return rooms;
        }

        public IEnumerable<ScenarioModel> GetScenarios()
        {
            List<ScenarioModel> scenario = new()
            {
                new() 
                {
                    Id = "1",
                    Name = "Good Morning",
                    IsActive = true,
                    ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0),
                    ScenarioActions = new List<ScenarioActionModel>()
                    {
                        new()
                        {
                            Name = "Fan",
                            Action = "Set temperature 18ºC",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0),
                            Type = "Fan"
                        },
                        new()
                        {
                            Name = "Shades",
                            Action = "Turn On",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 10),
                            Type = "Shades"
                        },
                        new()
                        {
                            Name = "HomePod",
                            Action = "Turn On",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 15),
                            Type = "Podcast"
                        },
                    }
                },
                new() 
                {
                    Id = "2",
                    Name = "Good Evening", 
                    IsActive = false,
                    ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0),
                    ScenarioActions = new List<ScenarioActionModel>()
                    {
                        new()
                        {
                            Name = "Fan",
                            Action = "Set temperature 20ºC",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0),
                            Type = "Fan"
                        },
                        new()
                        {
                            Name = "Light",
                            Action = "Turn On",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0),
                            Type = "Light"
                        },
                        new()
                        {
                            Name = "Shades",
                            Action = "Turn On",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 45, 0),
                            Type = "Shades"
                        },
                        new()
                        {
                            Name = "HomePod",
                            Action = "Turn Off",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 5),
                            Type = "Podcast"
                        },
                    }
                },
                new() 
                {
                    Id = "3",
                    Name = "Friday Night",
                    IsActive = false,
                    ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0),
                    ScenarioActions = new List<ScenarioActionModel>()
                    {
                        new()
                        {
                            Name = "Light",
                            Action = "Turn On",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0),
                            Type = "Light"
                        },
                        new()
                        {
                            Name = "Fan",
                            Action = "Set temperature 19ºC",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0),
                            Type = "Fan"
                        },
                        new()
                        {
                            Name = "Shades",
                            Action = "Turn Off",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0),
                            Type = "Shades"
                        },
                        new()
                        {
                            Name = "Unknown",
                            Action = "Turn Off",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0),
                            Type = "Unknown"
                        },
                        new()
                        {
                            Name = "HomePod",
                            Action = "Turn Off",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 5),
                            Type = "Podcast"
                        },
                    }
                },
                new() 
                {
                    Id = "4",
                    Name = "Welcome",
                    IsActive = true,
                    ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0),
                    ScenarioActions = new List<ScenarioActionModel>()
                    {
                        new()
                        {
                            Name = "Light",
                            Action = "Turn On",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0),
                            Type = "Light"
                        },
                        new()
                        {
                            Name = "Fan",
                            Action = "Set temperature 21ºC",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0),
                            Type = "Fan"
                        },
                        new()
                        {
                            Name = "Shades",
                            Action = "Turn On",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 30, 0),
                            Type = "Shades"
                        },
                    }
                },
                new() 
                {
                    Id = "5",
                    Name = "No one's home",
                    IsActive = true,
                    ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                    ScenarioActions = new List<ScenarioActionModel>()
                    {
                        new()
                        {
                            Name = "Light",
                            Action = "Turn Off",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                            Type = "Light"
                        },
                        new()
                        {
                            Name = "Fan",
                            Action = "Set temperature 15ºC",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                            Type = "Fan"
                        },
                        new()
                        {
                            Name = "Shades",
                            Action = "Turn Off",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 30, 0),
                            Type = "Shades"
                        },
                        new()
                        {
                            Name = "HomePod",
                            Action = "Turn Off",
                            ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 5),
                            Type = "Podcast"
                        },
                    }
                },
            };

            return scenario;
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

