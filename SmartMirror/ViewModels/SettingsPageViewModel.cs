using Prism.Services;
using SmartMirror.Services.Aqara;
using SmartMirror.Views;
using System.ComponentModel;
using static Android.Icu.Text.CaseMap;
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
        private readonly IAqaraService _aqaraService;
        private readonly IDialogService _dialogService;
        private readonly IMapperService _mapperService;
        private readonly IScenariosService _scenariosService;
        private readonly IDialogService _dialogService;
        private readonly ICamerasService _camerasService;

        private IEnumerable<ImageAndTitleBindableModel> _allScenarios;
        private IEnumerable<ImageAndTitleBindableModel> _allCameras;
        private IEnumerable<SettingsProvidersBindableModel> _allProviders;
        
        private CategoryBindableModel _providersCategory;
        private IDialogResult _dialogResult;

        public SettingsPageViewModel(
            IScenariosService scenariosService,
            IMapperService mapperService,
            IDialogService dialogService,
            ICamerasService camerasService,
            IAqaraService aqaraService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _aqaraService = aqaraService;
            _scenariosService = scenariosService;
            _mapperService = mapperService;
            _dialogService = dialogService;
            _camerasService = camerasService;
        }

        #region -- Public properties --

        private EPageState _pageState;
        public EPageState PageState
        {
            get => _pageState;
            set => SetProperty(ref _pageState, value);
        }

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

        private ICommand _loginWithAqaraCommand;
        public ICommand LoginWithAqaraCommand => _loginWithAqaraCommand ??= SingleExecutionCommand.FromFunc<SettingsProvidersBindableModel>(OnLoginWithAqaraCommandAsync);

        private ICommand _loginWithAppleCommand;
        public ICommand LoginWithAppleCommand => _loginWithAppleCommand ??= SingleExecutionCommand.FromFunc<SettingsProvidersBindableModel>(OnLoginWithAppleCommandAsync);

        private ICommand _loginWithAmazonCommand;
        public ICommand LoginWithAmazonCommand => _loginWithAmazonCommand ??= SingleExecutionCommand.FromFunc<SettingsProvidersBindableModel>(OnLoginWithAmazonCommandAsync);

        private ICommand _loginWithGoogleCommand;
        public ICommand LoginWithGoogleCommand => _loginWithGoogleCommand ??= SingleExecutionCommand.FromFunc<SettingsProvidersBindableModel>(OnLoginWithGoogleCommandAsync);

        private ICommand _selectCategoryCommand;
        public ICommand SelectCategoryCommand => _selectCategoryCommand ??= SingleExecutionCommand.FromFunc<CategoryBindableModel>(OnSelectCategoryCommandAsync);

        private ICommand _showScenarioSettingsCommand;
        public ICommand ShowScenarioSettingsCommand => _showScenarioSettingsCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnShowScenarioSettingsCommandAsync);

        private ICommand _showCameraSettingsCommand;
        public ICommand ShowCameraSettingsCommand => _showCameraSettingsCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnShowCameraSettingsCommandAsync);

        private ICommand _addNewCameraCommand;
        public ICommand AddNewCameraCommand => _addNewCameraCommand ??= SingleExecutionCommand.FromFunc(OnAddNewCameraCommandAsync);

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

                case ECategoryType.Providers:
                    CategoryElements = new(_allProviders);

                    DataState = CategoryElements.Any()
                        ? EPageState.Complete
                        : EPageState.Empty;
                    
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
            
            CreateProviders();

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
                    vm.TapCommand = ShowScenarioSettingsCommand;
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
                    vm.TapCommand = ShowCameraSettingsCommand;
                });

                var addCameraItem = new ImageAndTitleBindableModel()
                {
                    Name = Strings.AddNewCamera,
                    Type = ECategoryType.Cameras,
                    ImageSource = "subtract_plus",
                    TapCommand = AddNewCameraCommand,
                };

                var firstItems = new[] { addCameraItem };

                _allCameras = firstItems.Concat(_allCameras);

                var cameraCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Cameras);

                cameraCategory.Count = _allCameras.Count() - 1;
            }
        }

        private void CreateProviders()
        {
            _allProviders = new List<SettingsProvidersBindableModel>()
            {
                new SettingsProvidersBindableModel
                {
                    ImageSource = "aqara_logo",
                    Title = Strings.Connect,
                    AuthType = EAuthType.Aqara,
                    Type = ECategoryType.Providers,
                    IsConnected = _aqaraService.IsAuthorized,
                    TapCommand = LoginWithAqaraCommand,
                },

                new SettingsProvidersBindableModel
                {
                    ImageSource = "apple_logo",
                    Title = Strings.Connect,
                    AuthType = EAuthType.Apple,
                    Type = ECategoryType.Providers,
                    IsConnected = false,
                    TapCommand = LoginWithAppleCommand,
                },

                new SettingsProvidersBindableModel
                {
                    ImageSource = "amazon_logo",
                    Title = Strings.Connect,
                    AuthType = EAuthType.Amazon,
                    Type = ECategoryType.Providers,
                    IsConnected = false,
                    TapCommand = LoginWithAmazonCommand,
                },

                new SettingsProvidersBindableModel
                {
                    ImageSource = "google_logo",
                    Title = Strings.Connect,
                    AuthType = EAuthType.Google,
                    Type = ECategoryType.Providers,
                    IsConnected = false,
                    TapCommand = LoginWithGoogleCommand,
                },
            };

            _providersCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Providers);

            _providersCategory.Count = GetConnectedProviders();
        }

        private Task OnTryAgainCommandAsync()
        {
            PageState = EPageState.NoInternetLoader;

            return LoadAllDataAsync();
        }

        private Task OnShowScenarioSettingsCommandAsync(ImageAndTitleBindableModel scenario)
        {
            return _dialogService.ShowDialogAsync(nameof(ScenarioSettingsDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.SCENARIO, scenario },
            });
        }

        private async Task OnShowCameraSettingsCommandAsync(ImageAndTitleBindableModel camera)
        {
            var dialogResult = await _dialogService.ShowDialogAsync(nameof(CameraSettingsDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.CAMERA, camera },
            });

            if (dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.SHOW_CONFIRM_DIALOG, out bool confirm) && confirm)
            {
                dialogResult = await _dialogService.ShowDialogAsync(nameof(ConfirmDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure },
                    { Constants.DialogsParameterKeys.DESCRIPTION, Strings.TheCameraWillBeRemoved },
                });
            }
        }

        private Task OnAddNewCameraCommandAsync()
        {
            return _dialogService.ShowDialogAsync(nameof(AddNewCameraDialog), new DialogParameters()
            {
                { Constants.DialogsParameterKeys.TITLE, Strings.NewCamera },
            });
        }

        private Task OnCloseSettingsCommandAsync()
        {
            return NavigationService.GoBackAsync();
        }

        private async Task<bool> ShowLogoutConfirmationDialog(SettingsProvidersBindableModel settingsProvider)
        {
            var isSuccess = false;

            var dialogParameters = new DialogParameters();

            var providerName = settingsProvider.AuthType switch
            {
                EAuthType.Amazon => Strings.Amazon,
                EAuthType.Aqara => Strings.Aqara,
                EAuthType.Apple => Strings.Apple,
                EAuthType.Google => Strings.Google,

                _ => string.Empty,
            };

            dialogParameters.Add(Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure);
            dialogParameters.Add(Constants.DialogsParameterKeys.DESCRIPTION, $"{providerName} {Strings.WillBeDisconnected}");

            _dialogResult = await _dialogService.ShowDialogAsync(nameof(ConfirmDialog), dialogParameters);

            if (_dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool result))
            {
                isSuccess = result;
            }

            return isSuccess;
        }

        private async Task OnLoginWithAqaraCommandAsync(SettingsProvidersBindableModel settingsProvider)
        {
            if (_aqaraService.IsAuthorized)
            {
                var dialogResult = await ShowLogoutConfirmationDialog(settingsProvider);

                if (dialogResult)
                {
                    _aqaraService.LogoutFromAqara();
                }
            }
            else if (IsInternetConnected)
            {
                var testEmail = "botheadworks@gmail.com";
                var resultOfSendingCodeToMail = await _aqaraService.SendLoginCodeAsync(testEmail);

                if (resultOfSendingCodeToMail.IsSuccess)
                {
                    _dialogResult = await _dialogService.ShowDialogAsync(nameof(EnterCodeDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, Strings.Aqara },
                        { Constants.DialogsParameterKeys.AUTH_TYPE, settingsProvider.AuthType },
                    });
                }
                else
                {
                    _dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                        { Constants.DialogsParameterKeys.DESCRIPTION, resultOfSendingCodeToMail.Result?.MsgDetails ?? resultOfSendingCodeToMail.Message },
                    });
                }
            }
            else
            {
                _dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.NoInternetConnection}" },
                });
            }

            settingsProvider.IsConnected = _aqaraService.IsAuthorized;
            _providersCategory.Count = GetConnectedProviders();
        }

        private Task OnLoginWithAppleCommandAsync(SettingsProvidersBindableModel settingsProvider)
        {
            //TODO Implement login and logout from Apple

            DisplayNotImplementedDialog();

            _providersCategory.Count = GetConnectedProviders();

            return Task.CompletedTask;
        }

        private Task OnLoginWithAmazonCommandAsync(SettingsProvidersBindableModel settingsProvider)
        {
            //TODO Implement login and logout from Amazon

            DisplayNotImplementedDialog();

            _providersCategory.Count = GetConnectedProviders();

            return Task.CompletedTask;
        }

        private Task OnLoginWithGoogleCommandAsync(SettingsProvidersBindableModel settingsProvider)
        {
            //TODO Implement login and logout from Google

            DisplayNotImplementedDialog();

            _providersCategory.Count = GetConnectedProviders();

            return Task.CompletedTask;
        }

        private void DisplayNotImplementedDialog()
        {
            _dialogService.ShowDialog(nameof(ErrorDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.TITLE, "It's not available right now" },
                { Constants.DialogsParameterKeys.DESCRIPTION, $"Coming soon" },
            });
        }

        private int GetConnectedProviders()
        {
            return _allProviders.Count(x => x.IsConnected);
        }

        #endregion
    }
}
