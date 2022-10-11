using SmartMirror.Enums;
using SmartMirror.Models;
using Device = SmartMirror.Models.Device;
using NotificationModel = SmartMirror.Models.NotificationModel;

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

        public IEnumerable<Room> GetRooms()
        {
            List<Room> rooms = new()
            {
                new() { Name = "Dining Room", Description = "1 Accessories", Devices = GetDevices().Take(1) },
                new() { Name = "Downstairs", Description = "2 Accessories",Devices = GetDevices().Skip(1) },
                new() { Name = "Front Door", Description = "2 Accessories", Devices = GetDevices().Take(2) },
                new() { Name = "Garage", Description = "6 Accessories", Devices = GetDevices().Concat(GetDevices()) },
            };

            return rooms;
        }

        public IEnumerable<ScenarioModel> GetScenarios()
        {
            List<ScenarioModel> scenario = new()
            {
                new() 
                {
                    Name = "Good Morning",
                    IsActive = true,
                    IsFavorite = true, 
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
                    Name = "Good Evening", 
                    IsActive = false, 
                    IsFavorite = true, 
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
                    Name = "Friday Night",
                    IsActive = false,
                    IsFavorite = true,
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

        public IEnumerable<Device> GetDevices()
        {
            List<Device> devices = new()
            {
                new() { Name = "Garage Door", Type = "GarageDoor", Status = EDeviceStatus.On },
                new() { Name = "Front Door",  Type = "FrontDoor", Status = EDeviceStatus.On },
                new FanDevice { Name = "Fan", Type = "Fan", RoomName = "Living Room" , Status = EDeviceStatus.On, Power = "68%" }
            };

            return devices;
        }
    }
}

