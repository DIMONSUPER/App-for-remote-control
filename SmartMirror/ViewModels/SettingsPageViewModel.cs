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

        private IEnumerable<ImageAndTitleBindableModel> _allScenarios;
        private IEnumerable<SettingsProvidersBindableModel> _allProviders;
        private CategoryBindableModel _providersCategory;
        private IDialogResult _dialogResult;

        public SettingsPageViewModel(
            IScenariosService scenariosService,
            IMapperService mapperService,
            IDialogService dialogService,
            IAqaraService aqaraService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _aqaraService = aqaraService;
            _scenariosService = scenariosService;
            _mapperService = mapperService;
            _dialogService = dialogService;
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

        private ICommand _showScenarioDescriptionCommand;
        public ICommand ShowScenarioDescriptionCommand => _showScenarioDescriptionCommand ??= SingleExecutionCommand.FromFunc<ImageAndTitleBindableModel>(OnShowScenarioDescriptionCommandAsync);

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

                    DataState = CategoryElements.Any()
                        ? EPageState.Complete
                        : EPageState.Empty;

                    break;

                case ECategoryType.Providers:
                    CategoryElements = new(_allProviders);

                    DataState = CategoryElements.Any()
                        ? EPageState.Complete
                        : EPageState.Empty;
                    
                    break;

                default:
                    CategoryElements = new();
                    DataState = EPageState.Empty;
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task LoadAllDataAsync()
        {
            await LoadAllScenariosAsync();

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
                    vm.TapCommand = ShowScenarioDescriptionCommand;
                });

                var scenarioCategory = Categories.FirstOrDefault(category => category.Type == ECategoryType.Scenarios);

                scenarioCategory.Count = _allScenarios.Count();
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

        private async Task<bool> ShowLogoutConfirmationDialog(SettingsProvidersBindableModel settingsProvider)
        {
            var isSuccess = false;

            var dialogParameters = new DialogParameters();

            switch (settingsProvider.AuthType)
            {
                case EAuthType.Amazon:
                    {
                        dialogParameters.Add(Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure);
                        dialogParameters.Add(Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.Amazon} {Strings.WillBeDisconnected}");
                        break;
                    }
                case EAuthType.Aqara:
                    {
                        dialogParameters.Add(Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure);
                        dialogParameters.Add(Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.Aqara} {Strings.WillBeDisconnected}");
                        break;
                    }
                case EAuthType.Apple:
                    {
                        dialogParameters.Add(Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure);
                        dialogParameters.Add(Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.Apple} {Strings.WillBeDisconnected}");
                        break;
                    }
                case EAuthType.Google:
                    {
                        dialogParameters.Add(Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure);
                        dialogParameters.Add(Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.Google} {Strings.WillBeDisconnected}");
                        break;
                    }
            }

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
            else
            {
                if (IsInternetConnected)
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

                    await ProcessDialogResultAsync(_dialogResult, testEmail);
                }
                else
                {
                    _dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                        { Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.NoInternetConnection}" },
                    });
                }
            }

            settingsProvider.IsConnected = _aqaraService.IsAuthorized;
            _providersCategory.Count = GetConnectedProviders();
        }

        private async Task OnLoginWithAppleCommandAsync(SettingsProvidersBindableModel settingsProvider)
        {
            if (settingsProvider.IsConnected)
            {
                var dialogResult = await ShowLogoutConfirmationDialog(settingsProvider);

                if (dialogResult)
                {
                    //TODO Logout from Apple
                    settingsProvider.IsConnected = !settingsProvider.IsConnected;
                }
            }
            else
            {
                //TODO Loggin to Apple
                settingsProvider.IsConnected = !settingsProvider.IsConnected;
            }

            _providersCategory.Count = GetConnectedProviders();
        }

        private async Task OnLoginWithAmazonCommandAsync(SettingsProvidersBindableModel settingsProvider)
        {
            if (settingsProvider.IsConnected)
            {
                var dialogResult = await ShowLogoutConfirmationDialog(settingsProvider);

                if (dialogResult)
                {
                    //TODO Logout from Amazon
                    settingsProvider.IsConnected = !settingsProvider.IsConnected;
                }
            }
            else
            {
                //TODO Loggin to Amazon
                settingsProvider.IsConnected = !settingsProvider.IsConnected;
            }

            _providersCategory.Count = GetConnectedProviders();
        }

        private async Task OnLoginWithGoogleCommandAsync(SettingsProvidersBindableModel settingsProvider)
        {
            if (settingsProvider.IsConnected)
            {
                var dialogResult = await ShowLogoutConfirmationDialog(settingsProvider);

                if (dialogResult)
                {
                    //TODO Logout from Google
                    settingsProvider.IsConnected = !settingsProvider.IsConnected;
                }
            }
            else
            {
                //TODO Loggin to Google
                settingsProvider.IsConnected = !settingsProvider.IsConnected;
            }

            _providersCategory.Count = GetConnectedProviders();
        }

        private async Task ProcessDialogResultAsync(IDialogResult dialogResult, string email)
        {
            if (dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool result))
            {
                //await NavigationService.CreateBuilder()
                //    .AddSegment<MainTabbedPage>()
                //    .NavigateAsync();
            }
        }

        private int GetConnectedProviders()
        {
            return _allProviders.Count(x => x.IsConnected.Equals(true));
        }

        #endregion
    }
}
