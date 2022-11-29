using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Mock
{
    public class MockService : IMockService
    {
        #region -- IMockService implementation --

        public IEnumerable<AutomationBindableModel> GetAutomation()
        {
            return new List<AutomationBindableModel>
            {
                new AutomationBindableModel
                {
                    Name = "Door and Window Sensor",
                    IsReceiveNotifications = true,
                },
                new AutomationBindableModel
                {
                    Name = "Smart Plug",
                    IsReceiveNotifications = false,
                },
                new AutomationBindableModel
                {
                    Name = "Wall Switch (With Neutral, Single Rocker)",
                    IsReceiveNotifications = false,
                },
                new AutomationBindableModel
                {
                    Name = "LED Light Bulb",
                    IsReceiveNotifications = true,
                },
            };
        } 

        #endregion
    }
}
