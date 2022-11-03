using SmartMirror.Enums;
using SmartMirror.Interfaces;
using System.Windows.Input;

namespace SmartMirror.Models.BindableModels
{
    public class ImageAndTitleBindableModel : BindableBase, ICategoryElementModel, ITappable
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

        private object _model;
        public object Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
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

        #region -- ITappable implementation --

        private ICommand _tapCommand;
        public ICommand TapCommand
        {
            get => _tapCommand;
            set => SetProperty(ref _tapCommand, value);
        }

        #endregion
    }
}
