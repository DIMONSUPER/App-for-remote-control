using System.ComponentModel;
using System.Windows.Input;
using SmartMirror.Resources.Strings;

namespace SmartMirror.Models.BindableModels
{
    public class RoomBindableModel : BindableBase
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

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get => _createTime;
            set => SetProperty(ref _createTime, value);
        }

        private string _description = $"0 {Strings.Accessories}";
        public string Description
        {
            get => _description;
            private set => SetProperty(ref _description, value);
        }

        private int _devicesCount;
        public int DevicesCount
        {
            get => _devicesCount;
            set => SetProperty(ref _devicesCount, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private ICommand _tappedCommand;
        public ICommand TappedCommand
        {
            get => _tappedCommand;
            set => SetProperty(ref _tappedCommand, value);
        }

        private ICommand _selectedCommand;
        public ICommand SelectedCommand
        {
            get => _selectedCommand;
            set => SetProperty(ref _selectedCommand, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(DevicesCount))
            {
                Description = $"{DevicesCount} {Strings.Accessories}";
            }
        }

        #endregion
    }
}

