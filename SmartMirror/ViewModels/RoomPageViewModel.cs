using SmartMirror.Services.Mapper;

namespace SmartMirror.ViewModels;

public class RoomPageViewModel : BaseViewModel
{
    private readonly IMapperService _mapperService;

    public RoomPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService)
        : base(navigationService)
    {
        _mapperService = mapperService;
    }

    #region -- Public properties --

    #endregion
}

