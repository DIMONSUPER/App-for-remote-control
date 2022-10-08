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

    #region -- Overrides --

    public override async void Initialize(INavigationParameters parameters)
    {
        base.Initialize(parameters);

        await RefreshCamerasAsync();
    }

    #endregion

    #region -- Private helpers --

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
        var resultOfGettingCameras = await _camerasService.GetCamerasAsync();

        if (resultOfGettingCameras.IsSuccess)
        {
            var cameras = await _mapperService.MapRangeAsync<CameraBindableModel>(resultOfGettingCameras.Result, (m, vm) =>
            {
                vm.TapCommand = SelectCameraCommand;
            });

            Cameras = new(cameras);

            SelectCamera(Cameras.FirstOrDefault());
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
