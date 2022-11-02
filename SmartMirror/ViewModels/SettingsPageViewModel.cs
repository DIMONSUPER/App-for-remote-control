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
        private readonly IDevicesService _devicesService;
        private readonly IScenariosService _scenariosService;

        private IEnumerable<ImageAndTitleBindableModel> _allScenarios;
        private IEnumerable<ImageAndTitleBindableModel> _allAccessories;

        public SettingsPageViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IMapperService mapperService,
            IDevicesService devicesService,
            IScenariosService scenariosService)
            : base(navigationService)
        {
            _mapperService = mapperService;
            _dialogService = dialogService;
            _devicesService = devicesService;
            _scenariosService = scenariosService;
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

        private ICommand _tryAgainCommand;
        public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

        private ICommand _closeSettingsCommand;
        public ICommand CloseSettingsCommand => _closeSettingsCommand ??= SingleExecutionCommand.FromFunc(OnCloseSettingsCommandAsync);

        private ICommand _openAccessorySettingsCommand;
        public ICommand OpenAccessorySettingsCommand => _openAccessorySettingsCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnOpenAccessorySettingsCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            LoadCategories();

            await LoadAllDataAsync();

            SelectCategory(Categories.FirstOrDefault());
            SetElementsSelectedCategory();
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
            SetElementsSelectedCategory();

            return Task.CompletedTask;
        }

        private void SetElementsSelectedCategory()
        {
            if (SelectedCategory is not null)
            {
                switch (SelectedCategory.Type)
                {
                    case ECategoryType.Accessories:
                        CategoryElements = new(_allAccessories);
                        
                        DataState = _allAccessories.Any()
                            ? EPageState.Complete
                            : EPageState.None;

                        break;

                    case ECategoryType.Scenarios:
                        CategoryElements = new(_allScenarios);

                        DataState = CategoryElements.Any()
                            ? EPageState.Complete
                            : EPageState.Empty;

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
            var resultOfGettingAllDevices = await _devicesService.DownloadAllDevicesWithSubInfoAsync();
            
            if (resultOfGettingAllDevices.IsSuccess)
            {
                _allAccessories = _mapperService.MapRange<ImageAndTitleBindableModel>(_devicesService.AllSupportedDevices, (m, vm) =>
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
                    vm.Type = ECategoryType.Scenarios;
                    vm.ImageSource = "play_gray";
                    vm.TapCommand = ShowScenarioDescriptionCommand;
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

        private Task OnOpenAccessorySettingsCommandAsync(ImageAndTitleBindableModel accessory)
        {
            return _dialogService.ShowDialogAsync(nameof(AccessorySettingsDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.ACCESSORY, accessory },
            });
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
