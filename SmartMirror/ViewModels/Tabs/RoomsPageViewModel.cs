using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using SmartMirror.Services.Rooms;
using SmartMirror.ViewModels.Dialogs;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class RoomsPageViewModel : BaseTabViewModel
{
    private readonly ISmartHomeMockService _smartHomeMockService;
    private readonly IMapperService _mapperService;
    private readonly IAqaraService _aqaraService;
    private readonly IDialogService _dialogService;
    private readonly IRoomsService _roomsService;
    private readonly IDevicesService _devicesService;

    public RoomsPageViewModel(
        ISmartHomeMockService smartHomeMockService,
        INavigationService navigationService,
        IMapperService mapperService,
        IAqaraService aqaraService,
        IRoomsService roomsService,
        IDialogService dialogService,
        IDevicesService devicesService)
        : base(navigationService)
    {
        _smartHomeMockService = smartHomeMockService;
        _mapperService = mapperService;
        _aqaraService = aqaraService;
        _dialogService = dialogService;
        _roomsService = roomsService;
        _devicesService = devicesService;

        IsAqaraLoginButtonVisible = !_aqaraService.IsAuthorized;
        Title = "Rooms";
    }

    #region -- Public properties --

    private ICommand _roomTappedCommand;
    public ICommand RoomTappedCommand => _roomTappedCommand ??= SingleExecutionCommand.FromFunc<RoomBindableModel>(OnRoomTappedCommandAsync);

    private ICommand _accessorieTappedCommand;
    public ICommand AccessorieTappedCommand => _accessorieTappedCommand ??= new DelegateCommand<DeviceBindableModel>(async d => await OnAccessorieTappedCommandAsync(d));

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

    private ObservableCollection<DeviceBindableModel> _favoriteAccessories = new();
    public ObservableCollection<DeviceBindableModel> FavoriteAccessories
    {
        get => _favoriteAccessories;
        set => SetProperty(ref _favoriteAccessories, value);
    }

    private ObservableCollection<RoomBindableModel> _rooms = new();
    public ObservableCollection<RoomBindableModel> Rooms
    {
        get => _rooms;
        set => SetProperty(ref _rooms, value);
    }

    #endregion

    #region -- Overrides --

    public override async void OnAppearing()
    {
        base.OnAppearing();

        if (!IsDataLoading)
        {
            DataState = EPageState.Loading;

            await LoadRoomsAndDevicesAndChangeStateAsync();
        }
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            if (!IsDataLoading && DataState != EPageState.Complete)
            {
                DataState = EPageState.Loading;

                await LoadRoomsAndDevicesAndChangeStateAsync();
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        } 
    }

    #endregion

    #region -- Private helpers --

    private async Task OnAccessorieTappedCommandAsync(DeviceBindableModel device)
    {
        if (string.IsNullOrWhiteSpace(device.DeviceId) && device.Status != EDeviceStatus.Disconnected && !device.IsExecuting)
        {
            //Mocked device
            device.IsExecuting = true;

            await Task.Delay(500);

            device.Status = device.Status == EDeviceStatus.On ? EDeviceStatus.Off : EDeviceStatus.On;

            device.IsExecuting = false;
        }
        else if (device.DeviceType == EDeviceType.Switcher && device.Status != EDeviceStatus.Disconnected && !device.IsExecuting)
        {
            //Real device
            device.IsExecuting = true;

            var value = device.Status == EDeviceStatus.On ? "0" : "1";

            var updateResponse = await _devicesService.UpdateAttributeValueAsync(device.DeviceId, (device.EditableResourceId, value));

            if (updateResponse.IsSuccess)
            {
                device.Status = device.Status == EDeviceStatus.On ? EDeviceStatus.Off : EDeviceStatus.On;
            }
            else
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, updateResponse.Result?.Message },
                    { Constants.DialogsParameterKeys.DESCRIPTION, updateResponse.Result?.MsgDetails },
                });
            }

            device.IsExecuting = false;
        }
    }

    private async Task OnAqaraLoginButtonTappedAsync()
    {
        if (IsInternetConnected)
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
                    { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, sendLoginResponse.Result?.MsgDetails ?? sendLoginResponse.Message },
                });
            }

            await ProcessDialogResultAsync(dialogResult, testEmail);

            IsAqaraLoginButtonVisible = !_aqaraService.IsAuthorized;
        }
        else
        {
            //TODO: notify
        }
    }

    private async Task ProcessDialogResultAsync(IDialogResult dialogResult, string email)
    {
        if (dialogResult.Parameters.TryGetValue(nameof(TemporaryDialogViewModel.CodeText), out string code))
        {
            DataState = EPageState.Loading;

            var loginWithCodeResponse = await _aqaraService.LoginWithCodeAsync(email, code);

            if (loginWithCodeResponse.IsSuccess)
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "Success!" }
                });

                if (!IsDataLoading)
                {                 
                    await LoadRoomsAndDevicesAndChangeStateAsync();
                }
            }
            else
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "Fail!" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, loginWithCodeResponse.Message }
                });

                DataState = EPageState.Complete;
            }
        }
    }

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.RepeatAsync(LoadRoomsAndDevicesAsync, executionTime);
            
            if (IsInternetConnected)
            {
                DataState = isDataLoaded
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
    }

    private async Task LoadRoomsAndDevicesAndChangeStateAsync()
    {
        if (IsInternetConnected)
        {
            var isDataLoaded = await LoadRoomsAndDevicesAsync();

            if (IsInternetConnected)
            {
                DataState = isDataLoaded
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private async Task<bool> LoadRoomsAndDevicesAsync()
    {
        bool isLoaded = false;
        
        if (IsInternetConnected)
        {
            await Task.Delay(4000);

            var devices = _mapperService.MapRange<DeviceBindableModel>(_smartHomeMockService.GetDevices(), (m, vm) =>
            {
                vm.TappedCommand = AccessorieTappedCommand;
            });

            FavoriteAccessories = new(devices);

            await LoadDevicesAsync();

            var resultOfGettingRooms = await _roomsService.GetAllRoomsAsync();

            if (resultOfGettingRooms.IsSuccess)
            {
                var rooms = _mapperService.MapRange<RoomBindableModel>(resultOfGettingRooms.Result, (m, vm) =>
                {
                    vm.TappedCommand = RoomTappedCommand;
                });

                Rooms = new(rooms);

                isLoaded = true;
            } 
        }

        return isLoaded;
    }

    private async Task LoadDevicesAsync()
    {
        var devices = Enumerable.Empty<DeviceBindableModel>();

        if (_aqaraService.IsAuthorized)
        {
            var aqaraDevicesResponse = await _devicesService.DownloadAllDevicesWithSubInfoAsync();

            if (aqaraDevicesResponse.IsSuccess)
            {
                devices = _devicesService.AllSupportedDevices;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Can't download devices: {aqaraDevicesResponse.Message}");
            }
        }
        else
        {
            devices = _mapperService.MapRange<DeviceBindableModel>(_smartHomeMockService.GetDevices());
        }

        foreach(var device in devices)
        {
            device.TappedCommand = AccessorieTappedCommand;
        }

        FavoriteAccessories = new(devices);
    }

    private Task OnRoomTappedCommandAsync(RoomBindableModel room)
    {
        DataState = EPageState.Loading;

        return NavigationService.CreateBuilder()
            .AddSegment<RoomDetailsPageViewModel>(false)
            .AddParameter(KnownNavigationParameters.Animated, true)
            .AddParameter(nameof(Rooms), Rooms)
            .AddParameter(nameof(RoomBindableModel), room)
            .NavigateAsync().ContinueWith(x => DataState = EPageState.Complete);
    }

    #endregion
}

