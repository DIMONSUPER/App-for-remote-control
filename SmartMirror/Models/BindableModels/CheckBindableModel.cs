using System;
using System.Windows.Input;
using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
	public class CheckBindableModel : BindableBase, IChipModel
	{
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

