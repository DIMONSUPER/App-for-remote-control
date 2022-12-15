using System;
using System.Windows.Input;
using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
	public class RoomSourceBindableModel : BindableBase, IChipModel
	{
        #region -- Public properties --

        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        #endregion

        #region -- IChipModel implementation --

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private ICommand _tapCommand;
        public ICommand TapCommand
        {
            get => _tapCommand;
            set => SetProperty(ref _tapCommand, value);
        }

        #endregion
    }
}