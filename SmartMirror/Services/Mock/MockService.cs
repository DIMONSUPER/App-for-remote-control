using System;
using System.Collections.Generic;
using SmartMirror.Enums;
using SmartMirror.Models;
using Device = SmartMirror.Models.Device;

namespace SmartMirror.Services.Mock
{
    public class MockService :IMockService
    {
        public MockService()
        {
        }

        public List<Camera> GetCameras()
        {
            return new()
            {
                new() { Name = "Front Door 1", IsConnected = true, CreateTime = DateTime.Now },
                new() { Name = "Front Door 2", IsConnected = true, CreateTime = DateTime.Now },
                new() { Name = "Back Door 1", IsConnected = false },
                new() { Name = "Back Door 2", IsConnected = true, CreateTime = DateTime.Now },
                new() { Name = "Garage", IsConnected = false },
            };
        }

        public List<Notification> GetNotifications()
        {
            return new()
            {
                new() { Name = "Balcony Door" },
                new() { Name = "Upstairs Hallway Movement" },
                new() { Name = "Balcony Door" },
                new() { Name = "Balcony Door" },
            };
        }

        public List<Room> GetRooms()
        {
            return new()
            {
                new() { Name = "Dining Room", Description = "5 Accessories" },
                new() { Name = "Downstairs", Description = "4 Accessories" },
                new() { Name = "Front Door", Description = "2 Accessories" },
                new() { Name = "Garage", Description = "12 Accessories" },
            };
        }

        public List<Scenario> GetScenarios()
        {
            return new()
            {
                new() { Name = "Good Morning", IsActive = true, IsFavorite = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Good Evening", IsActive = false, IsFavorite = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0) },
                new() { Name = "Friday Night", IsActive = false, IsFavorite = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0) },
                new() { Name = "Good Morning", IsActive = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Good Morning", IsActive = false, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Good Morning", IsActive = true, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Good Morning", IsActive = false, ScenarioActions = GetScenariosActions(), ActivationTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
            };
        }

        public List<ScenarioAction> GetScenariosActions()
        {
            return new()
            {
                new() { Name = "Fan", Action = "Set temperature 16ºC", ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Shades", Action = "Turn OnC", ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "Light", Action = "Turn Off", ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
                new() { Name = "HomePod", Action = "Turn On", ActionTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) },
            };
        }

        public List<Device> GetDevices()
        {
            return new()
            {
                new() { Name = "Garage Door", Type = "GarageDoor", Status = EDeviceStatus.On },
                new() { Name = "Front Door",  Type = "Front DoorStatus", Status = EDeviceStatus.On },
                new() { Name = "Fan", Type = "Fan", Status = EDeviceStatus.On },
            };
            //{
            //    new() { Name = "Garage Door", Type = "GarageDoor", IsEnabled = true },
            //    new() { Name = "Front Door", Type = "FrontDoor", IsEnabled = true },
            //    new() { Name = "Fan", Type = "Fan", IsEnabled = true, RoomName = "Living Room", Description = "68%" },
            //};
        }
    }
}

