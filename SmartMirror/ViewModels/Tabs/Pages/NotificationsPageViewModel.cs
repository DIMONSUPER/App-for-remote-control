using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Notifications;
using SmartMirror.Services.Rooms;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartMirror.Resources.Strings;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class NotificationsPageViewModel : BaseTabViewModel
{
    private readonly INotificationsService _notificationsService;
    private readonly IDialogService _dialogService;
    private readonly IMapperService _mapperService;
    private readonly IDevicesService _devicesService;
    private readonly IRoomsService _roomsService;

    private List<NotificationSourceBindableModel> _roomsNotificationSource;
    private List<NotificationSourceBindableModel> _deviceNotificationSource;
    private List<NotificationGroupItemBindableModel> _allNotifications = new();

    public NotificationsPageViewModel(
        INotificationsService notificationsService,
        IDialogService dialogService,
        IMapperService mapperService,
        INavigationService navigationService,
        IDevicesService devicesService, 
        IRoomsService roomsService)
        : base(navigationService)
    {
        _notificationsService = notificationsService;
        _dialogService = dialogService;
        _mapperService = mapperService;
        _devicesService = devicesService;
        _roomsService = roomsService; 
        
        Title = "Notifications";
        DataState = EPageState.LoadingSkeleton;

        _devicesService.AllDevicesChanged += OnAllDevicesChanged;
        _notificationsService.AllNotificationsChanged += OnNotificationReceived;
        _roomsService.AllRoomsChanged += OnAllRoomsChanged;

        NotificationCategories = new()
        {
            Strings.Rooms,
            Strings.Accessories,
        };

        SelectedNotificationCategory = NotificationCategories.FirstOrDefault();
    }

    #region -- Public properties --

    private ObservableCollection<string> _notificationCategories;
    public ObservableCollection<string> NotificationCategories
    {
        get => _notificationCategories;
        set => SetProperty(ref _notificationCategories, value);
    }

    private string _selectedNotificationCategory;
    public string SelectedNotificationCategory
    {
        get => _selectedNotificationCategory;
        set => SetProperty(ref _selectedNotificationCategory, value);
    }

    private ObservableCollection<NotificationSourceBindableModel> _notificationsSources;
    public ObservableCollection<NotificationSourceBindableModel> NotificationsSources
    {
        get => _notificationsSources;
        set => SetProperty(ref _notificationsSources, value);
    }

    private NotificationSourceBindableModel _selectedNotificationSource;
    public NotificationSourceBindableModel SelectedNotificationSource
    {
        get => _selectedNotificationSource;
        set => SetProperty(ref _selectedNotificationSource, value);
    }

    private ObservableCollection<INotificationGroupItemModel> _notifications;
    public ObservableCollection<INotificationGroupItemModel> Notifications
    {
        get => _notifications;
        set => SetProperty(ref _notifications, value);
    }

    private bool _isNotificationsRefreshing;
    public bool IsNotificationsRefreshing
    {
        get => _isNotificationsRefreshing;
        set => SetProperty(ref _isNotificationsRefreshing, value);
    }

    private EPageState _notificationsState;
    public EPageState NotificationsState
    {
        get => _notificationsState;
        set => SetProperty(ref _notificationsState, value);
    }

    private ICommand _selectNotificationSourceCommand;
    public ICommand SelectNotificationSourceCommand => _selectNotificationSourceCommand ??= SingleExecutionCommand.FromFunc<NotificationSourceBindableModel>(OnSelectNotificationSourceCommandAsync);

    private ICommand _selectedNotificationCategoryChangedCommand;
    public ICommand SelectedNotificationCategoryChangedCommand => _selectedNotificationCategoryChangedCommand ??= SingleExecutionCommand.FromFunc(OnSelectedNotificationCategoryChangedCommandAsync);

    private ICommand _refreshNotificationsCommand;
    public ICommand RefreshNotificationsCommand => _refreshNotificationsCommand ??= SingleExecutionCommand.FromFunc(OnRefreshNotificationsCommandAsync, delayMillisec: 0);

    #endregion

    #region -- Overrides --

    public override void Destroy()
    {
        _devicesService.AllDevicesChanged -= OnAllDevicesChanged;
        _notificationsService.AllNotificationsChanged -= OnNotificationReceived;
        _roomsService.AllRoomsChanged -= OnAllRoomsChanged;

        base.Destroy();
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (IsInternetConnected)
        {
            if (!IsDataLoading && DataState != EPageState.Complete)
            {
                DataState = EPageState.LoadingSkeleton;

                await UpdateAllDataAndChangeStateAsync();
            } 
        }
        else
        {
            DataState = EPageState.NoInternet;

            IsNotificationsRefreshing = false;
        }
    }

    #endregion

    #region -- Private helpers --

    private int SelectedNotificationCategoryIndex => NotificationCategories.IndexOf(SelectedNotificationCategory);

    private async void OnNotificationReceived(object sender, NotificationGroupItemBindableModel notification)
    {
        await UpdateAllDataAndChangeStateAsync();
    }

    private async void OnAllRoomsChanged(object sender, EventArgs e)
    {
        await UpdateAllDataAndChangeStateAsync();
    }

    private async void OnAllDevicesChanged(object sender, EventArgs e)
    {
        await UpdateAllDataAndChangeStateAsync();
    }

    private async Task UpdateAllDataAndChangeStateAsync()
    {
        if (IsInternetConnected)
        {
            await LoadAllNotificationsAsync();

            await LoadNotificationSourcesAndApplyFilterAsync();
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private async Task LoadNotificationSourcesAndApplyFilterAsync()
    {
        if (_allNotifications.Any())
        {
            await Task.WhenAll(
                LoadRoomsNotificationSourcesAsync(),
                LoadDevicesNotificationSourcesAsync());

            SetNotificationSources();

            FilterNotifications();

            DataState = EPageState.Complete;
        }
        else
        {
            DataState = EPageState.Empty;
        }
    }

    private Task OnSelectedNotificationCategoryChangedCommandAsync()
    {
        SetNotificationSources();

        FilterNotifications();

        return Task.CompletedTask;
    }

    private Task OnSelectNotificationSourceCommandAsync(NotificationSourceBindableModel notificationSource)
    {
        SelectNotificationSource(notificationSource);

        FilterNotifications();

        return Task.CompletedTask;
    }

    private void SelectNotificationSource(NotificationSourceBindableModel notificationSource)
    {
        foreach (var source in NotificationsSources)
        {
            source.IsSelected = source.Id == notificationSource.Id;
        }

        SelectedNotificationSource = notificationSource;
    }

    private async Task LoadRoomsNotificationSourcesAsync()
    {
        var roomsNotificationSource = new List<NotificationSourceBindableModel>();

        var allRooms = await _roomsService.GetAllRoomsAsync();

        var notificationsFromRooms = _allNotifications.GroupBy(x => x.Device.PositionId);

        foreach (var room in allRooms)
        {
            var notifications = notificationsFromRooms.FirstOrDefault(x => x.Key == room.Id);

            var notificationSource = _mapperService.Map<NotificationSourceBindableModel>(room, (v, vm) =>
            {
                vm.NotificationsCount = notifications?.Count() ?? 0;
                vm.SelectCommand = SelectNotificationSourceCommand;
            });

            roomsNotificationSource.Add(notificationSource);
        }

        _roomsNotificationSource = new(roomsNotificationSource);
        _roomsNotificationSource.Insert(0, new NotificationSourceBindableModel
        {
            Name = Strings.All,
            NotificationsCount = _allNotifications.Count,
            SelectCommand = SelectNotificationSourceCommand,
        });
    }

    private async Task LoadDevicesNotificationSourcesAsync()
    {
        var devicesNotificationSource = new List<NotificationSourceBindableModel>();

        var allDevices = await _devicesService.GetAllSupportedDevicesAsync();

        var notificationsFromDevices = _allNotifications.GroupBy(x => x.Device.FullDeviceId);

        foreach (var device in allDevices)
        {
            var notifications = notificationsFromDevices.FirstOrDefault(x => x.Key == device.FullDeviceId);

            var notificationSource = _mapperService.Map<NotificationSourceBindableModel>(device, (v, vm) =>
            {
                vm.NotificationsCount = notifications?.Count() ?? 0;
                vm.SelectCommand = SelectNotificationSourceCommand;
            });

            devicesNotificationSource.Add(notificationSource);
        }

        _deviceNotificationSource = new(devicesNotificationSource);
        _deviceNotificationSource.Insert(0, new NotificationSourceBindableModel
        {
            Name = Strings.All,
            NotificationsCount = _allNotifications.Count,
            SelectCommand = SelectNotificationSourceCommand,
        });
    }

    private void FilterNotifications()
    {
        var notifications = Enumerable.Empty<NotificationGroupItemBindableModel>();

        bool isAllRoomsOrAccessoriesSelected = string.IsNullOrEmpty(SelectedNotificationSource.Id);

        switch ((ENotificationFilter)SelectedNotificationCategoryIndex)
        {
            case ENotificationFilter.ByRooms:

                notifications = isAllRoomsOrAccessoriesSelected
                    ? _allNotifications
                    : _allNotifications.Where(x => x.Device.PositionId == SelectedNotificationSource.Id);

                foreach (var notification in notifications)
                {
                    notification.IsRoomNameVisible = isAllRoomsOrAccessoriesSelected;
                }

                break;

            case ENotificationFilter.ByAccessories:

                notifications = isAllRoomsOrAccessoriesSelected
                    ? _allNotifications
                    : _allNotifications.Where(x => x.Device.FullDeviceId == SelectedNotificationSource.Id);

                foreach (var notification in notifications)
                {
                    notification.IsRoomNameVisible = true;
                }

                break;
        }

        if (notifications.Any())
        {
            Notifications = new(GetNotificationGroups(notifications));
            NotificationsState = EPageState.Complete;
        }
        else
        {
            Notifications = new();
            NotificationsState = EPageState.Empty;
        }
    }

    private void SetNotificationSources()
    {
        NotificationsSources = (ENotificationFilter)SelectedNotificationCategoryIndex switch
        {
            ENotificationFilter.ByRooms => new(_roomsNotificationSource),
            ENotificationFilter.ByAccessories => new(_deviceNotificationSource),
            _ => new(_roomsNotificationSource),
        };

        var notificationSource = (SelectedNotificationSource is null)
            ? NotificationsSources.FirstOrDefault()
            : NotificationsSources.FirstOrDefault(x => x.Id == SelectedNotificationSource.Id) ?? NotificationsSources.FirstOrDefault();

        SelectNotificationSource(notificationSource);
    }

    private async Task OnRefreshNotificationsCommandAsync()
    {
        if (!IsDataLoading)
        {
            // TODO delete when the subscription starts working properly
            await _notificationsService.DownloadAllNotificationsAsync();

            await LoadAllNotificationsAsync();

            await LoadNotificationSourcesAndApplyFilterAsync();

            IsNotificationsRefreshing = false;
        }
    }

    private async Task<bool> LoadAllNotificationsAsync()
    {
        bool isLoaded = false;

        if (IsInternetConnected)
        {
            var notifications = await _notificationsService.GetAllNotificationsAsync();

            _allNotifications = new(notifications);

            isLoaded = notifications.Any();
        }

        return isLoaded;
    }

    private ObservableCollection<INotificationGroupItemModel> GetNotificationGroups(IEnumerable<NotificationGroupItemBindableModel> notifications)
    {
        var lastTitleGroup = string.Empty;

        var notificationGrouped = new ObservableCollection<INotificationGroupItemModel>();

        foreach (var notificafication in notifications)
        {
            var titleGroup = notificafication.LastActivityTime.ToString(Constants.Formats.DATE_FORMAT);

            if (lastTitleGroup != titleGroup)
            {
                notificationGrouped.Add(new NotificationGroupTitleBindableModel()
                {
                    Title = titleGroup,
                });

                lastTitleGroup = titleGroup;
            }

            notificationGrouped.Add(notificafication);
        }

        return notificationGrouped;
    }

    #endregion
}