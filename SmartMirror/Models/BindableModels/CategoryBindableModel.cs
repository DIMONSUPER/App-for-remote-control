using SmartMirror.Enums;
using SmartMirror.Interfaces;
using System.Windows.Input;

namespace SmartMirror.Models.BindableModels
{
    public class CategoryBindableModel : BindableBase, ITappable
    {
        #region -- Public properties --

        private ECategoryType _type;
        public ECategoryType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _count;
        public string Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        #endregion

        #region -- ITappable implementation

        private ICommand _tapCommand;
        public ICommand TapCommand
        {
            get => _tapCommand;
            set => SetProperty(ref _tapCommand, value);
        }

        #endregion
    }
}
