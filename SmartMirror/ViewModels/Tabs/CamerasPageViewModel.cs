using System;
using SmartMirror.Enums;
using SmartMirror.Extensions;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Mock;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class CamerasPageViewModel : BaseTabViewModel
{
    private readonly ISmartHomeMockService _smartHomeMockService;

    public CamerasPageViewModel(
        INavigationService navigationService,
        ISmartHomeMockService smartHomeMockService)
        : base(navigationService)
    {
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
        set 
        {
            if (_selectedCamera is not null && _selectedCamera != value)
            {
                _selectedCamera.IsSelected = false;
            }

            if (value is not null)
            {
                value.IsSelected = true;
            }

            SetProperty(ref _selectedCamera, value);
        }
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

    #endregion

    #region -- Private helpers --

    private Task OnSelectCameraCommandAsync(CameraBindableModel selectedCamera)
    {
        SelectedCamera = selectedCamera;

        return Task.CompletedTask;
    }

    private Task OnRefreshCamerasCommandAsync() => RefreshCameras();

    private async Task RefreshCameras()
    {
        var cameras = _smartHomeMockService.GetCameras();
        cameras = cameras.Concat(cameras);
        cameras = cameras.Concat(cameras);
        cameras = cameras.Concat(cameras);
        cameras = cameras.Concat(cameras);
        var bindableCameras = cameras.Select(x => x.ToCameraBindableModel(SelectCameraCommand));

        await Task.Delay(Constants.Limits.SERVER_RESPONSE_DELAY);

        IsCamerasRefreshing = false;

        Cameras = new(bindableCameras);
        SelectedCamera = Cameras.FirstOrDefault();
    } 

    #endregion
}
