using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Notifications;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Google;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Scenarios;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartMirror.Services.Automation;

namespace SmartMirror.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly IAqaraService _aqaraService;
        private readonly IDialogService _dialogService;
        private readonly IMapperService _mapperService;
        private readonly IDevicesService _devicesService;
        private readonly IScenariosService _scenariosService;
        private readonly ICamerasService _camerasService;
        private readonly IGoogleService _googleService;
        private readonly INotificationsService _notificationsService;
        private readonly IAutomationService _automationService;

        private IEnumerable<ImageAndTitleBindableModel> _allAccessories = Enumerable.Empty<ImageAndTitleBindableModel>();
        private IEnumerable<ImageAndTitleBindableModel> _allScenarios = Enumerable.Empty<ImageAndTitleBindableModel>();
        private IEnumerable<ImageAndTitleBindableModel> _allCameras = Enumerable.Empty<ImageAndTitleBindableModel>();
        private IEnumerable<SettingsProvidersBindableModel> _allProviders = Enumerable.Empty<SettingsProvidersBindableModel>();
        private IEnumerable<ImageAndTitleBindableModel> _allNotifications = Enumerable.Empty<ImageAndTitleBindableModel>();
        private IEnumerable<ImageAndTitleBindableModel> _allAutomations = Enumerable.Empty<ImageAndTitleBindableModel>();

        private CategoryBindableModel _providersCategory;
        private CategoryBindableModel _notificationsCategory;
        private IDialogResult _dialogResult;

        public SettingsPageViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IMapperService mapperService,
            IDevicesService devicesService,
            IScenariosService scenariosService,
            ICamerasService camerasService,
            IAqaraService aqaraService,
            INotificationsService notificationsService,
            IAutomationService automationService,
            IGoogleService googleService)
            : base(navigationService)
        {
            _aqaraService = aqaraService;
            _mapperService = mapperService;
            _dialogService = dialogService;
            _devicesService = devicesService;
            _scenariosService = scenariosService;
            _camerasService = camerasService;
            _googleService = googleService;
            _notificationsService = notificationsService;
            _automationService = automationService;

            PageState = EPageState.LoadingSkeleton;

            _devicesService.AllDevicesChanged += OnAllDevicesChanged;
            _scenariosService.AllScenariosChanged += OnAllScenariosChanged;
            _automationService.AllAutomationsChanged += OnAllAutomationsChanged;

            LoadCategories();
            SelectCategory(SelectedCategory ?? Categories.FirstOrDefault());
        }

        #region -- Public properties --

        private EPageState _pageState;
        public EPageState PageState
        {
            get => _pageState;
            set => SetProperty(ref _pageState, value);
        }

        public bool IsDataLoading => PageState
            is EPageState.Loading
            or EPageState.NoInternetLoader
            or EPageState.LoadingSkeleton;

        private CategoryBindableModel _selectedCategory;
        public CategoryBindableModel SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private bool _isAllowNotifications;
        public bool IsAllowNotifications
        {
            get => _isAllowNotifications;
            set => SetProperty(ref _isAllowNotifications, value);
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

        private ICommand _showAutomationSettingsCommand;
        public ICommand ShowAutomationSettingsCommand => _showAutomationSettingsCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnShowAutomationSettingsCommandAsync);

        private ICommand _addNewCameraCommand;
        public ICommand AddNewCameraCommand => _addNewCameraCommand ??= SingleExecutionCommand.FromFunc(OnAddNewCameraCommandAsync);

        private ICommand _tryAgainCommand;
        public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

        private ICommand _closeSettingsCommand;
        public ICommand CloseSettingsCommand => _closeSettingsCommand ??= SingleExecutionCommand.FromFunc(OnCloseSettingsCommandAsync);

        private ICommand _openAccessorySettingsCommand;
        public ICommand OpenAccessorySettingsCommand => _openAccessorySettingsCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnOpenAccessorySettingsCommandAsync);

        private ICommand _changeStatusReceivingNotificationCommand;
        public ICommand ChangeStatusReceivingNotificationCommand => _changeStatusReceivingNotificationCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnChangeStatusReceivingNotificationCommandAsync);

        private ICommand _changeAllowNotificationsCommand;
        public ICommand ChangeAllowNotificationsCommand => _changeAllowNotificationsCommand ??= SingleExecutionCommand.FromFunc(OnChangeAllowNotificationsCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Task.Run(async () =>
            {
                await LoadAllDataAndChangeStateAsync();
            });
        }

        public override void Destroy()
        {
            _devicesService.AllDevicesChanged -= OnAllDevicesChanged;
            _scenariosService.AllScenariosChanged -= OnAllScenariosChanged;
            _automationService.AllAutomationsChanged -= OnAllAutomationsChanged;

            base.Destroy();
        }

        protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (IsInternetConnected)
            {
                PageState = EPageState.LoadingSkeleton;

                await LoadAllDataAndChangeStateAsync();
            }
            else
            {
                PageState = EPageState.NoInternet;
            }
        }

        #endregion

        #region -- Private helpers --

        private async void OnAllScenariosChanged(object sender, EventArgs e)
        {
            await LoadAllScenariosAsync();
        }

        private async void OnAllDevicesChanged(object sender, EventArgs e)
        {
            await LoadAllDevicesAsync();
        }

        private async void OnAllAutomationsChanged(object sender, EventArgs e)
        {
            await LoadAllAutomationsAsync();
        }

        private void LoadCategories()
        {
            Categories = new()
            {
                new()
                {
                    Type = ECategoryType.Notifications,
                    Name = Strings.Notifications,
                    HasImage = false,
                    TapCommand = SelectCategoryCommand,
                },
                new()
                {
                    Type = ECategoryType.Accessories,
                    Name = Strings.Accessories,
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
                new()
                {
                    Type = ECategoryType.Automation,
                    Name = Strings.Automation,
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
                CategoryElements = SelectedCategory.Type switch
                {
                    ECategoryType.Accessories => new(_allAccessories),
                    ECategoryType.Scenarios => new(_allScenarios),
                    ECategoryType.Cameras => new(_allCameras),
                    ECategoryType.Providers => new(_allProviders),
                    ECategoryType.Notifications => IsAllowNotifications ? new(_allNotifications) : new(),
                    ECategoryType.Automation => new(_allAutomations),
                    _ => throw new NotImplementedException(),
                };

                if (!IsAllowNotifications && SelectedCategory.Type == ECategoryType.Notifications)
                {
                    DataState = EPageState.Complete;
                }
                else
                {
                    DataState = CategoryElements.Any()
                    ? EPageState.Complete
                    : EPageState.Empty;
                }
            }
        }

        private async Task LoadAllDataAndChangeStateAsync()
        {
            if (IsInternetConnected)
            {
                await LoadAllDataAsync();

                if (IsInternetConnected)
                {
                    PageState = EPageState.Complete;

                    SetElementsSelectedCategory();
                }
                else
                {
                    PageState = EPageState.NoInternet;
                }
            }
            else
            {
                PageState = EPageState.NoInternet;
            }
        }

        private async Task<bool> LoadAllDataAsync()
        {
            bool isLoaded = false;

            if (IsInternetConnected)
            {
                isLoaded = await LoadAllDevicesAsync();
                isLoaded &= await LoadAllScenariosAsync();
                isLoaded &= await LoadAllCamerasAsync();
                isLoaded &= await LoadAllNotificationsAsync();
                isLoaded &= await LoadAllAutomationsAsync();

                CreateProviders();
            }

            return isLoaded;
        }

        private async Task<bool> LoadAllDevicesAsync()
        {
            var devices = await _devicesService.GetAllSupportedDevicesAsync();

            _allAccessories = _mapperService.MapRange<ImageAndTitleBindableModel>(devices, (m, vm) =>
            {
                vm.Model = m;
                vm.Type = ECategoryType.Accessories;
                vm.TapCommand = OpenAccessorySettingsCommand;
            });

            var deviceCategory = Categories.FirstOrDefault(c => c.Type == ECategoryType.Accessories);

            deviceCategory.Count = _allAccessories.Count().ToString();

            return true;
        }

        private async Task<bool> LoadAllScenariosAsync()
        {
            var scenarios = await _scenariosService.GetAllScenariosAsync();

            _allScenarios = _mapperService.MapRange<ImageAndTitleBindableModel>(scenarios, (m, vm) =>
            {
                vm.Model = m;
                vm.Type = ECategoryType.Scenarios;
                vm.ImageSource = Resources.IconsNames.clarity_pause_solid;
                vm.TapCommand = ShowScenarioSettingsCommand;
            });

            var scenarioCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Scenarios);

            scenarioCategory.Count = _allScenarios.Count().ToString();

            return true;
        }

        private async Task<bool> LoadAllAutomationsAsync()
        {
            var automations = await _automationService.GetAllAutomationsAsync();

            if (automations.Any())
            {
                _allAutomations = _mapperService.MapRange<ImageAndTitleBindableModel>(automations, (m, vm) =>
                {
                    vm.Model = m;
                    vm.Type = ECategoryType.Automation;
                    vm.ImageSource = "subtract_play_automation_small";
                    vm.TapCommand = ShowAutomationSettingsCommand;
                });
            }

            var automationCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Automation);

            automationCategory.Count = (_allAutomations.Count()).ToString();

            return true;
        }

        private async Task<bool> LoadAllCamerasAsync()
        {
            var resultOfGettingCameras = await _camerasService.GetCamerasAsync();

            if (resultOfGettingCameras.IsSuccess)
            {
                _allCameras = _mapperService.MapRange<ImageAndTitleBindableModel>(resultOfGettingCameras.Result, (m, vm) =>
                {
                    vm.Model = m;
                    vm.Type = ECategoryType.Cameras;
                    vm.ImageSource = "video_fill_dark";
                    vm.TapCommand = ShowCameraSettingsCommand;
                });
            }

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

            cameraCategory.Count = (_allCameras.Count() - 1).ToString();

            return resultOfGettingCameras.IsSuccess;
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

        private async Task<bool> LoadAllNotificationsAsync()
        {
            var devices = await _devicesService.GetAllSupportedDevicesAsync();

            _allNotifications = _mapperService.MapRange<ImageAndTitleBindableModel>(devices, (m, vm) =>
            {
                vm.Model = m;
                vm.Type = ECategoryType.Notifications;
                vm.IsToggled = m is DeviceBindableModel device && device.IsReceiveNotifications;
                vm.TapCommand = ChangeStatusReceivingNotificationCommand;
            });

            IsAllowNotifications = _notificationsService.IsAllowNotifications;

            _notificationsCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Notifications);

            _notificationsCategory.Count = IsAllowNotifications ? Strings.On : Strings.Off;

            return true;
        }

        private async Task OnTryAgainCommandAsync()
        {
            if (!IsDataLoading)
            {
                PageState = EPageState.NoInternetLoader;

                var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

                var isDataLoaded = await TaskRepeater.RepeatAsync(LoadAllDataAsync, executionTime);

                if (IsInternetConnected)
                {
                    PageState = EPageState.Complete;

                    SetElementsSelectedCategory();
                }
                else
                {
                    PageState = EPageState.NoInternet;
                }
            }
        }

        private Task OnChangeStatusReceivingNotificationCommandAsync(ImageAndTitleBindableModel accessory)
        {
            var device = accessory.Model as DeviceBindableModel;

            accessory.IsToggled = !accessory.IsToggled;
            device.IsReceiveNotifications = !device.IsReceiveNotifications;

            return _devicesService.UpdateDeviceAsync(device);
        }

        private Task OnChangeAllowNotificationsCommandAsync()
        {
            var state = !_notificationsService.IsAllowNotifications;

            IsAllowNotifications = state;

            _notificationsCategory.Count = IsAllowNotifications ? Strings.On : Strings.Off;

            SetElementsSelectedCategory();

            return _notificationsService.ChangeAllowNotificationsAsync(state);
        }

        private Task OnOpenAccessorySettingsCommandAsync(ImageAndTitleBindableModel accessory)
        {
            return _dialogService.ShowDialogAsync(nameof(AccessorySettingsDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.ACCESSORY, accessory },
            });
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

            if (dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool wasDeleted) && wasDeleted)
            {
                await LoadAllCamerasAsync();

                SetElementsSelectedCategory();
            }
        }

        private Task OnShowAutomationSettingsCommandAsync(ImageAndTitleBindableModel automation)
        {
            return _dialogService.ShowDialogAsync(nameof(AutomationSettingsDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.AUTOMATION, automation },
            });
        }

        private async Task OnAddNewCameraCommandAsync()
        {
            var dialogResult = await _dialogService.ShowDialogAsync(nameof(AddNewCameraDialog), new DialogParameters()
            {
                { Constants.DialogsParameterKeys.TITLE, Strings.NewCamera },
            });

            if (dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.IP_ADDRESS, out string ipAddress)
                && dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.LOGIN, out string login)
                && dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.PASSWORD, out string password))
            {
                var camera = new CameraBindableModel
                {
                    IpAddress = ipAddress,
                    Login = login,
                    Password = password,
                    CreateTime = DateTime.UtcNow,
                    Name = ipAddress,
                    IsShown = true,
                    VideoUrl = $"rtsp://{login}:{password}@{ipAddress}/live"
                };

                await _camerasService.UpdateCameraAsync(camera);

                await LoadAllCamerasAsync();

                SetElementsSelectedCategory();
            }
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
                    await _aqaraService.LogoutFromAqaraAsync();

                    await NavigationService.GoBackToRootAsync();
                }
            }
            else if (IsInternetConnected)
            {
                var resultOfSendingCodeToMail = await _aqaraService.SendLoginCodeAsync(Constants.Aqara.TEST_EMAIL);

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

        private async Task OnLoginWithGoogleCommandAsync(SettingsProvidersBindableModel settingsProvider)
        {
            var result = await _googleService.AutorizeAsync();

            if (result.IsSuccess)
            {
                //TODO: implement when have nest devices
            }
            else
            {
                //TODO: implement if needed
            }

            _providersCategory.Count = GetConnectedProviders();
        }

        private void DisplayNotImplementedDialog()
        {
            _dialogService.ShowDialog(nameof(ErrorDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.TITLE, "It's not available right now" },
                { Constants.DialogsParameterKeys.DESCRIPTION, $"Coming soon" },
            });
        }

        private string GetConnectedProviders()
        {
            return _allProviders.Count(x => x.IsConnected).ToString();
        }

        #endregion
    }
}
