using System;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Resources.DataTemplateSelectors
{
    public class ChipTemplateSelector : DataTemplateSelector
    {
        #region -- Public properties --

        public DataTemplate RoomSourceDataTemplate { get; set; }

        public DataTemplate ExpandButtonDataTemplate { get; set; }

        #endregion

        #region -- Overrides --

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            DataTemplate result = null;

            result = item.GetType().Name switch
            {
                nameof(RoomSourceBindableModel) => RoomSourceDataTemplate,
                nameof(SelectedBindableModel) => ExpandButtonDataTemplate,
                _ => null,
            };

            return result;
        }

        #endregion
    }
}