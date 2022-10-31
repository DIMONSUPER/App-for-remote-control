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

        private string _imageSource;
        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private ICommand _tapOnActionCommand;
        public ICommand TapOnActionCommand
        {
            get => _tapOnActionCommand;
            set => SetProperty(ref _tapOnActionCommand, value);
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
