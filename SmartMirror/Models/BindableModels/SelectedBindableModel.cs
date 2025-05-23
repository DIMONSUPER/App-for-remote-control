﻿using System;
using System.Windows.Input;
using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
    public class SelectedBindableModel : BindableBase, IBaseSelectableModel
    {
        #region -- IBaseSelectableModel implementation --

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