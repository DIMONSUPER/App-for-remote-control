using Prism.Services;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Aqara;
using SmartMirror.Views;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using static Android.Icu.Text.CaseMap;

namespace SmartMirror.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly IAqaraService _aqaraService;
        private readonly IDialogService _dialogService;

        private int _accountsConnectedCount;

        public SettingsPageViewModel(
            INavigationService navigationService,
            IAqaraService aqaraService,
            IDialogService dialogService)
            : base(navigationService)
        {
            _aqaraService = aqaraService;
            _dialogService = dialogService;
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

        private bool _isIsAqaraAccountConnected;
        public bool IsAqaraAccountConnected
        {
            get => _isIsAqaraAccountConnected;
            set => SetProperty(ref _isIsAqaraAccountConnected, value);
        }

        private bool _isAppleAccountConnected;
        public bool IsAppleAccountConnected
        {
            get => _isAppleAccountConnected;
            set => SetProperty(ref _isAppleAccountConnected, value);
        }

        private bool _isAmazonAccountConnected;
        public bool IsAmazonAccountConnected
        {
            get => _isAmazonAccountConnected;
            set => SetProperty(ref _isAmazonAccountConnected, value);
        }

        private bool _isGoogleAccountConnected;
        public bool IsGoogleAccountConnected
        {
            get => _isGoogleAccountConnected;
            set => SetProperty(ref _isGoogleAccountConnected, value);
        }

        private ICommand _loginWithAqaraCommand;
        public ICommand LoginWithAqaraCommand => _loginWithAqaraCommand ??= SingleExecutionCommand.FromFunc<EAuthType>(OnLoginWithAqaraCommandAsync);

        private ICommand _loginWithAppleCommand;
        public ICommand LoginWithAppleCommand => _loginWithAppleCommand ??= new DelegateCommand(OnLoginWithAppleCommandAsync);

        private ICommand _loginWithAmazonCommand;
        public ICommand LoginWithAmazonCommand => _loginWithAmazonCommand ??= SingleExecutionCommand.FromFunc<EAuthType>(OnLoginWithAmazonCommandAsync);

        private ICommand _loginWithGoogleCommand;
        public ICommand LoginWithGoogleCommand => _loginWithGoogleCommand ??= SingleExecutionCommand.FromFunc<EAuthType>(OnLoginWithGoogleCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            IsAqaraAccountConnected = _aqaraService.IsAuthorized;

            LoadCategories();

            DataState = EPageState.Complete;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if(args.PropertyName == nameof(IsAqaraAccountConnected)
                || args.PropertyName == nameof(IsAppleAccountConnected)
                || args.PropertyName == nameof(IsAmazonAccountConnected)
                || args.PropertyName == nameof(IsGoogleAccountConnected))
            {
                LoadCategories();
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
                    Count = GetConnectedProviders(),
                },
            };
        }

        private async Task OnLoginWithAqaraCommandAsync(EAuthType authType)
        {
            

            if (_aqaraService.IsAuthorized)
            {
                _dialogService.ShowDialog(nameof(ConfirmDialog), new DialogParameters
                {
                    {Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure },
                    { Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.Aqara} {Strings.WillBeDisconnected}" },
                }, r =>
                {
                    if (r.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool result) && result)
                    {
                        _aqaraService.LogoutFromAqara();
                    }
                });
            }
            else
            {
                IDialogResult dialogResult;

                if (IsInternetConnected)
                {
                    var testEmail = "botheadworks@gmail.com";
                    var resultOfSendingCodeToMail = await _aqaraService.SendLoginCodeAsync(testEmail);

                    if (resultOfSendingCodeToMail.IsSuccess)
                    {
                        dialogResult = await _dialogService.ShowDialogAsync(nameof(EnterCodeDialog), new DialogParameters
                        {
                            { Constants.DialogsParameterKeys.TITLE, Strings.Aqara },
                            { Constants.DialogsParameterKeys.AUTH_TYPE, authType },
                        });
                    }
                    else
                    {
                        dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                        {
                            { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                            { Constants.DialogsParameterKeys.DESCRIPTION, resultOfSendingCodeToMail.Result?.MsgDetails ?? resultOfSendingCodeToMail.Message },
                        });
                    }

                    await ProcessDialogResultAsync(dialogResult, testEmail);
                }
                else
                {
                    dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                        { Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.NoInternetConnection}" },
                    });
                }

                IsAqaraAccountConnected = _aqaraService.IsAuthorized;
            }
        }

        private void OnLoginWithAppleCommandAsync()
        {
            if (IsAppleAccountConnected)
            {
                _dialogService.ShowDialog(nameof(ConfirmDialog), new DialogParameters
                {
                    {Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure },
                    { Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.Apple} {Strings.WillBeDisconnected}" },
                }, r =>
                {
                    if (r.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool result) && result)
                    {
                        //TODO Logout from Apple
                        IsAppleAccountConnected = !IsAppleAccountConnected;
                    }
                });
            }
            else
            {
                IsAppleAccountConnected = !IsAppleAccountConnected;
            }
        }

        private Task OnLoginWithAmazonCommandAsync(EAuthType authType)
        {
            if (IsAmazonAccountConnected)
            {
                _dialogService.ShowDialog(nameof(ConfirmDialog), new DialogParameters
                {
                    {Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure },
                    { Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.Amazon} {Strings.WillBeDisconnected}" },
                }, r =>
                {
                    if (r.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool success) && success)
                    {
                        //TODO Logout from Amazon
                        IsAmazonAccountConnected = !IsAmazonAccountConnected;
                    }
                });
            }
            else
            {
                IsAmazonAccountConnected = !IsAmazonAccountConnected;
            }

            return Task.CompletedTask;
        }

        private Task OnLoginWithGoogleCommandAsync(EAuthType authType)
        {
            if (IsGoogleAccountConnected)
            {
                _dialogService.ShowDialog(nameof(ConfirmDialog), new DialogParameters
                {
                    {Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure },
                    { Constants.DialogsParameterKeys.DESCRIPTION, $"{Strings.Google} {Strings.WillBeDisconnected}" },
                }, r =>
                {
                    if (r.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool success) && success)
                    {
                        //TODO Logout from Google
                        IsGoogleAccountConnected = !IsGoogleAccountConnected;
                    }
                });
            }
            else
            {
                IsGoogleAccountConnected = !IsGoogleAccountConnected;
            }
            

            return Task.CompletedTask;
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
            var connectedAcconts = new List<bool>()
            {
                IsAppleAccountConnected,
                IsAqaraAccountConnected,
                IsAmazonAccountConnected,
                IsGoogleAccountConnected,
            };

            return connectedAcconts.Count(x => x.Equals(true));
        }

        #endregion
    }
}
