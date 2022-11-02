using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Scenarios;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly IMapperService _mapperService;
        private readonly IScenariosService _scenariosService;
        private readonly IDialogService _dialogService;
        private readonly ICamerasService _camerasService;

        private IEnumerable<ImageAndTitleBindableModel> _allScenarios;
        private IEnumerable<ImageAndTitleBindableModel> _allCameras;

        public SettingsPageViewModel(
            IScenariosService scenariosService,
            IMapperService mapperService,
            IDialogService dialogService,
            ICamerasService camerasService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _scenariosService = scenariosService;
            _mapperService = mapperService;
            _dialogService = dialogService;
            _camerasService = camerasService;
        }

        #region -- Public properties --

        private CategoryBindableModel _selectedCategory;
        public CategoryBindableModel SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private ObservableCollection<CategoryBindableModel> _categories;
        public ObservableCollection<CategoryBindableModel> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private ObservableCollection<ICategoryElementModel> _categoryElements = new();
        public ObservableCollection<ICategoryElementModel> CategoryElements
        {
            get => _categoryElements;
            set => SetProperty(ref _categoryElements, value);
        }

        private EPageState _pageState;
        public EPageState PageState
        {
            get => _pageState;
            set => SetProperty(ref _pageState, value);
        }

        private ICommand _selectCategoryCommand;
        public ICommand SelectCategoryCommand => _selectCategoryCommand ??= SingleExecutionCommand.FromFunc<CategoryBindableModel>(OnSelectCategoryCommandAsync);

        private ICommand _showScenarioDescriptionCommand;
        public ICommand ShowScenarioDescriptionCommand => _showScenarioDescriptionCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnShowScenarioDescriptionCommandAsync);

        private ICommand _showCameraDescriptionCommand;
        public ICommand ShowCameraDescriptionCommand => _showCameraDescriptionCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnShowScenarioDescriptionCommandAsync);

        private ICommand _tryAgainCommand;
        public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

        private ICommand _closeSettingsCommand;
        public ICommand CloseSettingsCommand => _closeSettingsCommand ??= SingleExecutionCommand.FromFunc(OnCloseSettingsCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            LoadCategories();

            await LoadAllDataAsync();

            //temporarily
            DataState = EPageState.Empty;
        }

        protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            base.OnConnectivityChanged(sender, e);

            if (IsInternetConnected)
            {
                await LoadAllDataAsync();
            }
            else
            {
                PageState = EPageState.NoInternet;
            }
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
                    IsSelected = true,
                    TapCommand = SelectCategoryCommand,
                },
                new()
                {
                    Type = ECategoryType.Scenarios,
                    Name = Strings.Scenarios,
                    TapCommand = SelectCategoryCommand,
                },
                new()
                {
                    Type = ECategoryType.Cameras,
                    Name = Strings.Cameras,
                    TapCommand = SelectCategoryCommand,
                },
                new()
                {
                    Type = ECategoryType.Providers,
                    Name = Strings.Providers,
                    TapCommand = SelectCategoryCommand,
                },
            };

            SelectedCategory = Categories.FirstOrDefault();
        }

        private void SelectCategory(CategoryBindableModel category)
        {
            if (SelectedCategory is not null)
            {
                SelectedCategory.IsSelected = false;
            }

            if (category is not null)
            {
                category.IsSelected = true;
            }

            SelectedCategory = category;
        }

        private Task OnSelectCategoryCommandAsync(CategoryBindableModel category)
        {
            SelectCategory(category);

            switch (category.Type)
            {
                case ECategoryType.Scenarios:
                    CategoryElements = new(_allScenarios);
                    break;
                case ECategoryType.Cameras:
                    CategoryElements = new(_allCameras);
                    break;
                default:
                    CategoryElements = new();
                    break;
            }

            DataState = CategoryElements.Any()
                ? EPageState.Complete
                : EPageState.Empty;

            return Task.CompletedTask;
        }

        private async Task LoadAllDataAsync()
        {
            await LoadAllScenariosAsync();

            await LoadAllCamerasAsync();

            PageState = EPageState.Complete;
        }

        private async Task LoadAllScenariosAsync()
        {
            var resultOfGettingAllScenarios = await _scenariosService.GetScenariosAsync();

            if (resultOfGettingAllScenarios.IsSuccess)
            {
                _allScenarios = _mapperService.MapRange<ImageAndTitleBindableModel>(resultOfGettingAllScenarios.Result, (m, vm) =>
                {
                    vm.Type = ECategoryType.Scenarios;
                    vm.ImageSource = "play_gray";
                    vm.TapCommand = ShowScenarioDescriptionCommand;
                });

                var scenarioCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Scenarios);

                scenarioCategory.Count = _allScenarios.Count();
            }
        }

        private async Task LoadAllCamerasAsync()
        {
            var resultOfGettingCameras = await _camerasService.GetCamerasAsync();

            if (resultOfGettingCameras.IsSuccess)
            {
                _allCameras = _mapperService.MapRange<ImageAndTitleBindableModel>(resultOfGettingCameras.Result, (m, vm) =>
                {
                    vm.Type = ECategoryType.Cameras;
                    vm.ImageSource = "video_fill_dark";
                    vm.TapCommand = ShowCameraDescriptionCommand;
                });

                var cameraCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Cameras);

                cameraCategory.Count = _allCameras.Count();
            }
        }

        private Task OnTryAgainCommandAsync()
        {
            PageState = EPageState.NoInternetLoader;

            return LoadAllDataAsync();
        }

        private Task OnShowScenarioDescriptionCommandAsync(ImageAndTitleBindableModel scenario)
        {
            return _dialogService.ShowDialogAsync(nameof(ScenarioDescriptionDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.SCENARIO, scenario },
            });
        }

        private Task OnCloseSettingsCommandAsync()
        {
            return NavigationService.GoBackAsync();
        }

        #endregion
    }
}
