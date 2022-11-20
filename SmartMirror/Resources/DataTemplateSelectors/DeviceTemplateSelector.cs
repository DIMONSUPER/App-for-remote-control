using SmartMirror.Enums;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.DataTemplates;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class DeviceTemplateSelector : DataTemplateSelector
    {
        #region -- Public properties --

        public DataTemplate DisconnectedDataTemplate { get; set; }

        public DataTemplate ConnectedDataTemplate { get; set; }

        public DataTemplate OnDataTemplate { get; set; }

        public DataTemplate OffDataTemplate { get; set; }

        #endregion

        #region -- Overrides --

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            DataTemplate result = null;

            if (item is DeviceBindableModel deviceBindable)
            {
                result = deviceBindable.Status switch
                {
                    EDeviceStatus.Disconnected => DisconnectedDataTemplate,
                    EDeviceStatus.Connected => ConnectedDataTemplate,
                    EDeviceStatus.On => OnDataTemplate,
                    EDeviceStatus.Off => OffDataTemplate,
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"{item.GetType()} is not supported by {nameof(DeviceTemplateSelector)}");
            }

            return result;
        }

        #endregion
    }
}

