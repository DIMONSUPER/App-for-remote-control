using System;
using System.Windows.Input;
using SmartMirror.Interfaces;
using SmartMirror.Enums;

namespace SmartMirror.Models.BindableModels
{
    public class SettingsProvidersBindableModel : BindableBase, ICategoryElementModel, ITappable
    {

        #region -- Public properties --

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        public EAuthType AuthType { get; set; }

        #endregion

        #region -- ITappable implementation --

        private ICommand _tapCommand;
        public ICommand TapCommand
        {
            get => _tapCommand;
            set => SetProperty(ref _tapCommand, value);
        }

        #endregion

        #region -- ICategoryElementModel implementation --

        private string _imageSource;
        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private ECategoryType _type;
        public ECategoryType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        #endregion
    }
}

