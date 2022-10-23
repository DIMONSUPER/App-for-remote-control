using CommunityToolkit.Maui.Alerts;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
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
    
    private ICommand _videoPaybackErrorCommand;
    public ICommand VideoPaybackErrorCommand => _videoPaybackErrorCommand ??= SingleExecutionCommand.FromFunc(OnVideoPaybackErrorCommandAsync);

    #endregion

    #region -- Overrides --

    public override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (!IsDataLoading)
        {
            DataState = EPageState.Loading;

            await LoadCamerasAndChangeStateAsync();
        }
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
            if (!IsDataLoading && DataState != EPageState.Complete)
            {
                DataState = EPageState.Loading;
                
                await LoadCamerasAndChangeStateAsync();
            }
        }
        else
        {
            IsCamerasRefreshing = false;
            VideoAction = EVideoAction.Pause;
            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private Task OnVideoPaybackErrorCommandAsync()
    {
        Toast.Make(Strings.CannotPlayVideo).Show();

        return Task.CompletedTask;
    }

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.RepeatAsync(LoadCamerasAsync, executionTime);

            if (IsInternetConnected)
            {
                (DataState, VideoAction) = isDataLoaded
                    ? (EPageState.Complete, EVideoAction.Play)
                    : (EPageState.Empty, EVideoAction.Pause);
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
    }

    private Task OnSelectCameraCommandAsync(CameraBindableModel selectedCamera)
    {
        SelectCamera(selectedCamera);

        return Task.CompletedTask;
    }

    private async Task OnRefreshCamerasCommandAsync() 
    {
        if (!IsDataLoading)
        {
            await LoadCamerasAndChangeStateAsync();

            IsCamerasRefreshing = false;
        }
    }

    private async Task LoadCamerasAndChangeStateAsync()
    {
        if (IsInternetConnected)
        {
            var isDataLoaded = await LoadCamerasAsync();

            if (IsInternetConnected)
            {
                (DataState, VideoAction) = isDataLoaded
                    ? (EPageState.Complete, EVideoAction.Play)
                    : (EPageState.Empty, EVideoAction.Pause);
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

    private async Task<bool> LoadCamerasAsync()
    {
        bool isLoaded = false;

        if (IsInternetConnected)
        {
            await Task.Delay(1000);

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

                isLoaded = true;
            } 
        }

        return isLoaded;
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
