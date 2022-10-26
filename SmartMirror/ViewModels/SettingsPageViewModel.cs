using SmartMirror.Enums;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using System.Collections.ObjectModel;

namespace SmartMirror.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        public SettingsPageViewModel(
            INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        private ObservableCollection<CategoryBindableModel> _categories;
        public ObservableCollection<CategoryBindableModel> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private CategoryBindableModel _selectedCategory;
        public CategoryBindableModel SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            LoadCategories();

            DataState = EPageState.Complete;
        }

        #endregion

        #region -- Private helpers --

        private void LoadCategories()
        {
            Categories = new()
            {
                new()
                {
                    Type = ECategoryType.Accessories,
                    Name = Strings.Accessories,
                    Count = 1,
                    IsSelected = true,
                },
                new()
                {
                    Type = ECategoryType.Scenarios,
                    Name = Strings.Scenarios,
                    Count = 15,
                },
                new()
                {
                    Type = ECategoryType.Cameras,
                    Name = Strings.Cameras,
                    Count = 13,
                },
                new()
                {
                    Type = ECategoryType.Providers,
                    Name = Strings.Providers,
                    Count = 8,
                },
            };
        }

        #endregion
    }
}
