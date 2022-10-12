using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartMirror.Services.Aqara;
using SmartMirror.Views.Dialogs;
using SmartMirror.ViewModels.Dialogs;
using SmartMirror.Models.BindableModels;

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

    private ICommand _accessorieTappedCommand;
    public ICommand AccessorieTappedCommand => _accessorieTappedCommand ??= SingleExecutionCommand.FromFunc<DeviceBindableModel>(OnAccessorieTappedCommandAsync);

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

    private ObservableCollection<DeviceBindableModel> _favoriteAccessories;
    public ObservableCollection<DeviceBindableModel> FavoriteAccessories
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

    public override async Task InitializeAsync(INavigationParameters parameters)
    {
        await base.InitializeAsync(parameters);

        await LoadRoomsAndDevicesAsync();
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            await LoadRoomsAndDevicesAsync();
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
        if (device.DeviceType == EDeviceType.Switcher)
        {
            var resp = await _aqaraService.ToggleTheLightsAsync(device.DeviceId);

            await Task.Delay(800);

            if (resp.IsSuccess)
            {
                var resp2 = await _aqaraService.GetDeviceAttributeValueAsync(device.DeviceId);

                if (resp2.IsSuccess)
                {
                    device.Status = resp2.Result?.FirstOrDefault()?.Value == "0" ? EDeviceStatus.Off : EDeviceStatus.On;
                }
            }
        }
    }

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

                await LoadRoomsAndDevicesAsync();
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

        await LoadRoomsAndDevicesAsync();
    }

    private async Task LoadRoomsAndDevicesAsync()
    {
        if (IsInternetConnected)
        {
            var devices = await _mapperService.MapRangeAsync<DeviceBindableModel>(_smartHomeMockService.GetDevices(), (m, vm) =>
            {
                vm.TappedCommand = AccessorieTappedCommand;
            });

            FavoriteAccessories = new(devices);

            await AddAqaraDevicesIfAuthorizedAsync();

            var rooms = await _mapperService.MapRangeAsync<RoomBindableModel>(_smartHomeMockService.GetRooms(), (m, vm) =>
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

    private async Task AddAqaraDevicesIfAuthorizedAsync()
    {
        if (_aqaraService.IsAuthorized)
        {
            var aqaraDevicesResponse = await _aqaraService.GetAllDevicesAsync();

            if (aqaraDevicesResponse.IsSuccess)
            {
                var devices = await _mapperService.MapRangeAsync<DeviceBindableModel>(aqaraDevicesResponse.Result.Devices, (m, vm) =>
                {
                    vm.TappedCommand = AccessorieTappedCommand;
                });

                foreach (var d in devices)
                {
                    if (d.Model.Contains("switch"))
                    {
                        d.DeviceType = EDeviceType.Switcher;
                        d.Type = "Lights";

                        if (d.State > 0)
                        {
                            var deviceAttributeResponse = await _aqaraService.GetDeviceAttributeValueAsync(d.DeviceId, "4.1.85");

                            if (deviceAttributeResponse.IsSuccess)
                            {
                                d.Status = deviceAttributeResponse.Result?.FirstOrDefault()?.Value == "0" ? EDeviceStatus.Off : EDeviceStatus.On;
                            }
                        }

                        FavoriteAccessories.Insert(0, d);
                    }
                    else if (d.Model.Contains("weather"))
                    {
                        d.DeviceType = EDeviceType.Sensor;
                        d.Type = "Humidity";

                        if (d.State > 0)
                        {
                            var deviceAttributeResponse = await _aqaraService.GetDeviceAttributeValueAsync(d.DeviceId, "0.2.85", "0.1.85", "0.3.85");

                            if (deviceAttributeResponse.IsSuccess)
                            {
                                d.Status = EDeviceStatus.Connected;

                                var humididty = double.Parse(deviceAttributeResponse.Result.FirstOrDefault(x => x.ResourceId == "0.2.85").Value) / 100 + "%";
                                var temperature = double.Parse(deviceAttributeResponse.Result.FirstOrDefault(x => x.ResourceId == "0.1.85").Value) / 100 + "℃";
                                var pressure = double.Parse(deviceAttributeResponse.Result.FirstOrDefault(x => x.ResourceId == "0.1.85").Value) / 1000 + "kPa";

                                var humidityDevice = new DeviceBindableModel()
                                {
                                    AdditionalInfo = humididty,
                                    Status = EDeviceStatus.Connected,
                                    DeviceType = EDeviceType.Sensor,
                                    Name = "Humidity",
                                    Type = "Humidity",
                                };

                                FavoriteAccessories.Insert(0, humidityDevice);

                                var temperatureDevice = new DeviceBindableModel()
                                {
                                    AdditionalInfo = temperature,
                                    Status = EDeviceStatus.Connected,
                                    DeviceType = EDeviceType.Sensor,
                                    Name = "Temperature",
                                    Type = "Temperature",
                                };

                                FavoriteAccessories.Insert(0, temperatureDevice);

                                var pressureDevice = new DeviceBindableModel()
                                {
                                    AdditionalInfo = pressure,
                                    Status = EDeviceStatus.Connected,
                                    DeviceType = EDeviceType.Sensor,
                                    Name = "Pressure",
                                    Type = "Pressure",
                                };

                                FavoriteAccessories.Insert(0, pressureDevice);
                            }
                        }
                    }
                    else
                    {
                        FavoriteAccessories.Insert(0, d);
                    }

                    System.Diagnostics.Debug.WriteLine($"{d.DeviceId}: {d.Model}, {d.ModelType}, {d.State}, {d.Name}, {d.Type}");
                }
            }
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

