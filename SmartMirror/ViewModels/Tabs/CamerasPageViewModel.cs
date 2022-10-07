using Android.Hardware.Camera2;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class CamerasPageViewModel : BaseTabViewModel
{
    private readonly ISmartHomeMockService _smartHomeMockService;
    private readonly IMapperService _mapperService;

    public CamerasPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService,
        ISmartHomeMockService smartHomeMockService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _smartHomeMockService = smartHomeMockService;
        
        DataState = EPageState.Complete;
    }

    #region -- Public properties --

    private ObservableCollection<CameraBindableModel> _cameras;
    public ObservableCollection<CameraBindableModel> Cameras
    {
        get => _cameras;
        set => SetProperty(ref _cameras, value);
    }

    private CameraBindableModel? _selectedCamera;
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

    private ICommand? _selectCameraCommand;
    public ICommand SelectCameraCommand => _selectCameraCommand ??= SingleExecutionCommand.FromFunc<CameraBindableModel>(OnSelectCameraCommandAsync);
    
    private ICommand? _refreshCamerasCommand;
    public ICommand RefreshCamerasCommand => _refreshCamerasCommand ??= SingleExecutionCommand.FromFunc(OnRefreshCamerasCommandAsync);

    #endregion

    #region -- Overries --

    public override async void Initialize(INavigationParameters parameters)
    {
        base.Initialize(parameters);

        await RefreshCameras();
    }


    public override void OnAppearing()
    {
        base.OnAppearing();

        if (Cameras.Any())
        {
            SelectCamera(Cameras.FirstOrDefault());
        }
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();

        SelectCamera(null);
    }

    #endregion

    #region -- Private helpers --

    private Task OnSelectCameraCommandAsync(CameraBindableModel selectedCamera)
    {
        SelectCamera(selectedCamera);

        return Task.CompletedTask;
    }

    private Task OnRefreshCamerasCommandAsync() => RefreshCameras();

    private async Task RefreshCameras()
    {
        var cameras = await _mapperService.MapRangeAsync<CameraBindableModel>(
            _smartHomeMockService.GetCameras(), (m, vm) =>
            {
                vm.TapCommand = SelectCameraCommand;
            });

        await Task.Delay(Constants.Limits.SERVER_RESPONSE_DELAY);

        IsCamerasRefreshing = false;

        Cameras = new(cameras);
    }

    private void SelectCamera(CameraBindableModel selectedCamera)
    {
        if (Cameras is not null && Cameras.Any())
        {
            foreach (var camera in Cameras)
            {
                if (camera == selectedCamera)
                {
                    SelectedCamera = camera;
                    camera.IsSelected = true;
                }
                else
                {
                    camera.IsSelected = false;
                }
            }
        }
    }

    #endregion
}
