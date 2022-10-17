using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Mapper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class CamerasPageViewModel : BaseTabViewModel
{
    private readonly IMapperService _mapperService;
    private readonly ICamerasService _camerasService;

    public CamerasPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService,
        ICamerasService camerasService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _camerasService = camerasService;

        Title = "Cameras";
    }

    #region -- Public properties --

    private ObservableCollection<CameraBindableModel> _cameras;
    public ObservableCollection<CameraBindableModel> Cameras
    {
        get => _cameras;
        set => SetProperty(ref _cameras, value);
    }

    private CameraBindableModel _selectedCamera;
    public CameraBindableModel SelectedCamera
    {
        get => _selectedCamera;
        set => SetProperty(ref _selectedCamera, value);
    }

    private bool _isCamerasRefreshing;
    public bool IsCamerasRefreshing
    {
        get => _isCamerasRefreshing;
        set => SetProperty(ref _isCamerasRefreshing, value);
    }

    private EVideoAction _videoAction;
    public EVideoAction VideoAction
    {
        get => _videoAction;
        set => SetProperty(ref _videoAction, value);
    }

    private EVideoLoadingState _videoLoadingState;
    public EVideoLoadingState VideoLoadingState
    {
        get => _videoLoadingState;
        set => SetProperty(ref _videoLoadingState, value);
    }

    private ICommand _selectCameraCommand;
    public ICommand SelectCameraCommand => _selectCameraCommand ??= SingleExecutionCommand.FromFunc<CameraBindableModel>(OnSelectCameraCommandAsync);
    
    private ICommand _refreshCamerasCommand;
    public ICommand RefreshCamerasCommand => _refreshCamerasCommand ??= SingleExecutionCommand.FromFunc(OnRefreshCamerasCommandAsync);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    #endregion

    #region -- Overrides --

    public override async void OnAppearing()
    {
        base.OnAppearing();

        DataState = EPageState.Loading;

        await RefreshCamerasAsync();

        VideoAction = EVideoAction.Play;
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();

        VideoAction = EVideoAction.Pause;
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            DataState = EPageState.Loading;

            await RefreshCamerasAsync();

            if (IsSelected)
            {
                VideoAction = EVideoAction.Play;
            }
        }
        else
        {
            if (IsSelected)
            {
                VideoAction = EVideoAction.Pause;
            }

            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private async Task OnTryAgainCommandAsync()
    {
        DataState = EPageState.NoInternetLoader;

        await RefreshCamerasAsync();
    }

    private Task OnSelectCameraCommandAsync(CameraBindableModel selectedCamera)
    {
        SelectCamera(selectedCamera);

        return Task.CompletedTask;
    }

    private async Task OnRefreshCamerasCommandAsync() 
    {
        await RefreshCamerasAsync();

        IsCamerasRefreshing = false;
    }

    private async Task RefreshCamerasAsync()
    {
        await Task.Delay(2000);

        if (IsInternetConnected)
        {
            var resultOfGettingCameras = await _camerasService.GetCamerasAsync();

            if (resultOfGettingCameras.IsSuccess)
            {
                var cameras = _mapperService.MapRange<CameraBindableModel>(resultOfGettingCameras.Result, (m, vm) =>
                {
                    vm.TapCommand = SelectCameraCommand;
                });

                Cameras = new(cameras);

                var camera = SelectedCamera is null
                    ? Cameras.FirstOrDefault()
                    : Cameras.FirstOrDefault(x => x.Id == SelectedCamera.Id) ?? Cameras.FirstOrDefault();

                SelectCamera(camera);

                DataState = EPageState.Complete;
            }
            else if(!IsInternetConnected)
            {
                DataState = EPageState.NoInternet;
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private void SelectCamera(CameraBindableModel selectedCamera)
    {
        if (SelectedCamera is not null)
        {
            SelectedCamera.IsSelected = false;
        }

        if (selectedCamera is not null)
        {
            selectedCamera.IsSelected = true;
        }

        SelectedCamera = selectedCamera;
    }

    #endregion
}
