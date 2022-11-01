using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Devices;
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
        private readonly IDialogService _dialogService;
        private readonly IDevicesService _deviceService;
        private readonly IScenariosService _scenariosService;

        private readonly ICommand _selectCategoryCommand;
        private readonly ICommand _showScenarioDescriptionCommand;

        private IEnumerable<ImageAndTitleBindableModel> _allAccessories;
        private IEnumerable<ImageAndTitleBindableModel> _allScenarios;

        public SettingsPageViewModel(
            IScenariosService scenariosService,
            IDevicesService devicesService,
            IMapperService mapperService,
            IDialogService dialogService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _deviceService = devicesService;
            _scenariosService = scenariosService;
            _mapperService = mapperService;
            _dialogService = dialogService;

            _selectCategoryCommand = SingleExecutionCommand.FromFunc<CategoryBindableModel>(OnSelectCategoryCommandAsync);
            _showScenarioDescriptionCommand = SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnShowScenarioDescriptionCommandAsync);
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

        private ICommand _tryAgainCommand;
        public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

        private ICommand _closeSettingsCommand;
        public ICommand CloseSettingsCommand => _closeSettingsCommand ??= SingleExecutionCommand.FromFunc(OnCloseSettingsCommandAsync);

        private ICommand _openAccessorySettingsCommand;
        public ICommand OpenAccessorySettingsCommand => _openAccessorySettingsCommand ??= SingleExecutionCommand.FromFunc(OnOpenAccessorySettingsCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            base.OnAppearing();

            LoadCategories();

            await LoadAllDataAsync();

            var category = Categories.FirstOrDefault();

            SelectCategory(category);
            SetCategoryElements(category);
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
                    TapCommand = _selectCategoryCommand,
                },
                new()
                {
                    Type = ECategoryType.Scenarios,
                    Name = Strings.Scenarios,
                    TapCommand = _selectCategoryCommand,
                },
                new()
                {
                    Type = ECategoryType.Cameras,
                    Name = Strings.Cameras,
                    TapCommand = _selectCategoryCommand,
                },
                new()
                {
                    Type = ECategoryType.Providers,
                    Name = Strings.Providers,
                    TapCommand = _selectCategoryCommand,
                },
            };
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

            SetCategoryElements(category);

            return Task.CompletedTask;
        }

        private void SetCategoryElements(CategoryBindableModel category)
        {
            if (category is not null)
            {
                switch (category.Type)
                {
                    case ECategoryType.Accessories:
                        DataState = _allAccessories.Any()
                            ? EPageState.Complete
                            : EPageState.None;

                        CategoryElements = new(_allAccessories);
                        break;

                    case ECategoryType.Scenarios:
                        DataState = _allScenarios.Count() == 0
                            ? EPageState.Empty
                            : EPageState.Complete;

                        CategoryElements = new(_allScenarios);
                        break;

                    default:
                        CategoryElements = new();
                        DataState = EPageState.Empty;
                        break;
                } 
            }
        }

        private async Task LoadAllDataAsync()
        {
            await Task.WhenAll(
                LoadAllDevicesAsync(),
                LoadAllScenariosAsync());

            PageState = EPageState.Complete;
        }

        private async Task LoadAllDevicesAsync()
        {
            var resultOfGettingAllDevices = await _deviceService.DownloadAllDevicesWithSubInfoAsync();

            if (resultOfGettingAllDevices.IsSuccess)
            {
                _allAccessories = _mapperService.MapRange<ImageAndTitleBindableModel>(_deviceService.AllSupportedDevices, (m, vm) =>
                {
                    vm.TapCommand = OpenAccessorySettingsCommand;
                });

                var deviceCategory = Categories.FirstOrDefault(c => c.Type == ECategoryType.Accessories);

                deviceCategory.Count = _allAccessories.Count();
            }
        }

        private async Task LoadAllScenariosAsync()
        {
            var resultOfGettingAllScenarios = await _scenariosService.GetScenariosAsync();

            if (resultOfGettingAllScenarios.IsSuccess)
            {
                _allScenarios = _mapperService.MapRange<ImageAndTitleBindableModel>(resultOfGettingAllScenarios.Result, (m, vm) =>
                {
                    vm.ImageSource = "play_gray";
                    vm.TapCommand = _showScenarioDescriptionCommand;
                });

                var scenarioCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Scenarios);

                scenarioCategory.Count = _allScenarios.Count();
            }
        }

        private Task OnTryAgainCommandAsync()
        {
            PageState = EPageState.NoInternetLoader;

            return LoadAllDataAsync();
        }

        private Task OnOpenAccessorySettingsCommandAsync()
        {
            return Task.CompletedTask;
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
