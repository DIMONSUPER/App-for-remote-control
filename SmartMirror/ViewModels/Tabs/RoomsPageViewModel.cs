using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Device = SmartMirror.Models.Device;
using SmartMirror.Services.Aqara;
using SmartMirror.Views.Dialogs;
using SmartMirror.Resources.Strings;
using SmartMirror.ViewModels.Dialogs;
using Prism.Services;

namespace SmartMirror.ViewModels.Tabs;

public class RoomsPageViewModel : BaseTabViewModel
{
    private readonly ISmartHomeMockService _smartHomeMockService;
    private readonly IMapperService _mapperService;
    private readonly IAqaraService _aqaraService;
    private readonly IDialogService _dialogService;

    public RoomsPageViewModel(
        ISmartHomeMockService smartHomeMockService,
        INavigationService navigationService,
        IMapperService mapperService,
        IAqaraService aqaraService,
        IDialogService dialogService)
        : base(navigationService)
    {
        _smartHomeMockService = smartHomeMockService;
        _mapperService = mapperService;
        _aqaraService = aqaraService;
        _dialogService = dialogService;

        IsAqaraLoginButtonVisible = !_aqaraService.IsAuthorized;

        Title = "Rooms";
        DataState = EPageState.Loading;
    }

    #region -- Public properties --

    private ICommand _roomTappedCommand;
    public ICommand RoomTappedCommand => _roomTappedCommand ??= SingleExecutionCommand.FromFunc<RoomBindableModel>(OnRoomTappedCommandAsync);

    private ICommand _aqaraLoginButtonTappedCommand;
    public ICommand AqaraLoginButtonTappedCommand => _aqaraLoginButtonTappedCommand ??= SingleExecutionCommand.FromFunc(OnAqaraLoginButtonTappedAsync);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    private bool _isAqaraLoginButtonVisible;
    public bool IsAqaraLoginButtonVisible
    {
        get => _isAqaraLoginButtonVisible;
        set => SetProperty(ref _isAqaraLoginButtonVisible, value);
    }

    private ObservableCollection<Device> _favoriteAccessories;
    public ObservableCollection<Device> FavoriteAccessories
    {
        get => _favoriteAccessories;
        set => SetProperty(ref _favoriteAccessories, value);
    }

    private ObservableCollection<RoomBindableModel> _rooms;
    public ObservableCollection<RoomBindableModel> Rooms
    {
        get => _rooms;
        set => SetProperty(ref _rooms, value);
    }

    #endregion

    #region -- Overrides --

    public override async void Initialize(INavigationParameters parameters)
    {
        base.Initialize(parameters);

        await LoadRoomsAsync();
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            await LoadRoomsAsync();
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private async Task OnAqaraLoginButtonTappedAsync()
    {
        var testEmail = "botheadworks@gmail.com";
        var sendLoginResponse = await _aqaraService.SendLoginCodeAsync(testEmail);

        IDialogResult dialogResult;

        if (sendLoginResponse.IsSuccess)
        {
            dialogResult = await _dialogService.ShowDialogAsync(nameof(TemporaryDialog));
        }
        else
        {
            dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.TITLE, sendLoginResponse.Result?.Message },
                { Constants.DialogsParameterKeys.DESCRIPTION, sendLoginResponse.Result?.MsgDetails },
            });
        }

        await ProcessDialogResultAsync(dialogResult, testEmail);

        IsAqaraLoginButtonVisible = !_aqaraService.IsAuthorized;
    }

    private async Task ProcessDialogResultAsync(IDialogResult dialogResult, string email)
    {
        if (dialogResult.Parameters.TryGetValue(nameof(TemporaryDialogViewModel.CodeText), out string code))
        {
            var loginWithCodeResponse = await _aqaraService.LoginWithCodeAsync(email, code);

            if (loginWithCodeResponse.IsSuccess)
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "Success!" }
                });

                DataState = EPageState.Loading;

                await LoadRoomsAsync();
            }
            else
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "Fail!" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, loginWithCodeResponse.Message }
                });
            }
        }
    }

    private async Task OnTryAgainCommandAsync()
    {
        DataState = EPageState.NoInternetLoader;

        await Task.Delay(1000);

        await LoadRoomsAsync();
    }

    private async Task LoadRoomsAsync()
    {
        if (IsInternetConnected)
        {
            FavoriteAccessories = new(_smartHomeMockService.GetDevices());

            var rooms = _mapperService.MapRange<RoomBindableModel>(_smartHomeMockService.GetRooms(), (m, vm) =>
            {
                vm.TappedCommand = RoomTappedCommand;
            });

            Rooms = new(rooms);

            DataState = EPageState.Complete;
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private Task OnRoomTappedCommandAsync(RoomBindableModel room)
    {
        return NavigationService.CreateBuilder()
            .AddSegment<RoomPageViewModel>(false)
            .AddParameter(KnownNavigationParameters.Animated, true)
            .AddParameter(nameof(Rooms), Rooms)
            .AddParameter(nameof(RoomBindableModel), room)
            .NavigateAsync();
    }

    #endregion
}

