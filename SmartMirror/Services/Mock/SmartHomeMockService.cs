using Android.Hardware.Display;
using SmartMirror.Enums;
using SmartMirror.Models;
using Device = SmartMirror.Models.Device;
using Notification = SmartMirror.Models.Notification;

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
                    Name = "Front Door 1", 
                    IsConnected = true,
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://archive.org/download/ElephantsDream/ed_hd_512kb.mp4",
                },
                new() 
                {
                    Name = "Front Door 2",
                    IsConnected = true, 
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://archive.org/download/BigBuckBunny_328/BigBuckBunny_512kb.mp4",
                },
                new() 
                { 
                    Name = "Back Door 1", 
                    IsConnected = false,
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://archive.org/download/Sintel/sintel-2048-stereo_512kb.mp4",
                },
                new() 
                {
                    Name = "Back Door 2", 
                    IsConnected = true, 
                    CreateTime = DateTime.Now,
                    VideoUrl = "https://archive.org/download/CatByExcessivelyLoudTvMjjxmykdfb3Mp4Cat/CatByExcessivelyLoudTvMjjxmykdfb3Mp4Cat.mp4",
                },
                new() 
                {
                    Name = "Garage",
                    CreateTime = DateTime.Now,
                    IsConnected = false,
                    VideoUrl = "https://archive.org/download/CatReachingOutMmhubokeakyMp4Cat/CatReachingOutMmhubokeakyMp4Cat.mp4",
                },
            };

            return cameras;
        }

        public IEnumerable<Notification> GetNotifications()
        {
            List<Notification> notifications = new()
            {
                new() { Name = "Balcony Door" },
                new() { Name = "Upstairs Hallway Movement" },
                new() { Name = "Balcony Door" },
                new() { Name = "Balcony Door" },
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

        public IEnumerable<Scenario> GetScenarios()
        {
            List<Scenario> scenario = new()
            {
                new() { Name = "Good Morning", IsActive = true, IsFavorite = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Good Evening", IsActive = false, IsFavorite = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0) },
                new() { Name = "Friday Night", IsActive = false, IsFavorite = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0) },
                new() { Name = "Good Morning", IsActive = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Good Morning", IsActive = false, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Good Morning", IsActive = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Good Morning", IsActive = false, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
            };

            return scenario;
        }

        public IEnumerable<ScenarioAction> GetScenariosActions()
        {
            List<ScenarioAction> scenarioAction = new()
            {
                new() { Name = "Fan", Action = "Set temperature 16ºC", ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Shades", Action = "Turn OnC", ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Light", Action = "Turn Off", ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "HomePod", Action = "Turn On", ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
            };

            return scenarioAction;
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

