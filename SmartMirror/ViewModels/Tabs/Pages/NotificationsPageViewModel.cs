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

namespace SmartMirror.ViewModels.Tabs.Pages;

public class NotificationsPageViewModel : BaseTabViewModel
{
    private readonly INotificationsService _notificationsService;
    private readonly IDialogService _dialogService;
    private readonly IMapperService _mapperService;
    private readonly IDevicesService _devicesService;
    private readonly IRoomsService _roomsService;

    private ObservableCollection<NotificationGroupItemBindableModel> _allNotifications;
    private ObservableCollection<NotificationSourceBindableModel> _roomsNotificationSource;
    private ObservableCollection<NotificationSourceBindableModel> _deviceeNotificationSource;

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
        _devicesService.AllDevicesChanged += OnAllDevicesChanged;
        _notificationsService.NotificationReceived += OnNotificationReceived;
        _roomsService.AllRoomsChanged += OnAllRoomsChanged;

        NotificationCategories = new()
        {
            LocalizationResourceManager.Instance["Rooms"] as string,
            LocalizationResourceManager.Instance["Accessories"] as string,
        };
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

    private int _selectedNotificationCategoryIndex;
    public int SelectedNotificationCategoryIndex
    {
        get => _selectedNotificationCategoryIndex;
        set => SetProperty(ref _selectedNotificationCategoryIndex, value);
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
    public ICommand SelectNotificationSourceCommand => _selectNotificationSourceCommand ??= 
        SingleExecutionCommand.FromFunc<NotificationSourceBindableModel>(OnSelectNotificationSourceCommandAsync, delayMillisec: 0);

    private ICommand _selectedNotificationCategoryChangedCommand;
    public ICommand SelectedNotificationCategoryChangedCommand => _selectedNotificationCategoryChangedCommand ??= SingleExecutionCommand.FromFunc(OnSelectedNotificationCategoryChangedCommandAsync);

    private ICommand _refreshNotificationsCommand;
    public ICommand RefreshNotificationsCommand => _refreshNotificationsCommand ??= SingleExecutionCommand.FromFunc(OnRefreshNotificationsCommandAsync, delayMillisec: 0);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    #endregion

    #region -- Overrides --

    public override void Destroy()
    {
        _devicesService.AllDevicesChanged -= OnAllDevicesChanged;
        _notificationsService.NotificationReceived -= OnNotificationReceived;

        base.Destroy();
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        base.OnConnectivityChanged(sender, e);

        IsNotificationsRefreshing = !IsInternetConnected;

        if (!IsDataLoading && DataState != EPageState.Complete)
        {
            await LoadAllDataAndChangeStateAsync(); 
        }
    }

    #endregion

    #region -- Private helpers --

    private async void OnNotificationReceived(object sender, NotificationGroupItemBindableModel notification)
    {
        await LoadAllDataAndChangeStateAsync();
    }

    private async void OnAllRoomsChanged(object sender, EventArgs e)
    {
        await LoadAllDataAndChangeStateAsync();
    }

    private async void OnAllDevicesChanged(object sender, EventArgs e)
    {
        await LoadAllDataAndChangeStateAsync();
    }

    private async Task LoadAllDataAndChangeStateAsync()
    {
        if (IsInternetConnected)
        {
            DataState = EPageState.LoadingSkeleton;

            await LoadAllNotificationsAsync();

            if (_allNotifications.Any())
            {
                await Task.WhenAll(
                    LoadRoomsNotificationSourcesAsync(), 
                    LoadDevicesNotificationSourcesAsync());

                SetAndSelectNotificationSources();

                FilterNotifications();

                DataState = EPageState.Complete;
            }
            else
            {
                DataState = EPageState.Empty;
            } 
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private Task OnSelectedNotificationCategoryChangedCommandAsync()
    {
        SetAndSelectNotificationSources();

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
        if (SelectedNotificationSource is not null)
        {
            SelectedNotificationSource.IsSelected = false;
        }

        if (notificationSource is not null)
        {
            notificationSource.IsSelected = true;
        }

        SelectedNotificationSource = notificationSource;
    }

    private async Task LoadRoomsNotificationSourcesAsync()
    {
        var roomsNotificationSource = new ObservableCollection<NotificationSourceBindableModel>();

        var allRooms = await _roomsService.GetAllRoomsAsync();

        var notificationsFromRooms = _allNotifications.GroupBy(x => x.Device.PositionId);

        foreach (var room in allRooms)
        {
            var notifications = notificationsFromRooms.FirstOrDefault(x => x.Key == room.Id);

            var notificationSource = new NotificationSourceBindableModel
            {
                Id = room.Id,
                Name = room.Name,
                NotificationsCount = notifications is null
                    ? 0
                    : notifications.Count(),
                SelectCommand = SelectNotificationSourceCommand,
            };

            roomsNotificationSource.Add(notificationSource);
        }

        _roomsNotificationSource = new(roomsNotificationSource);
        _roomsNotificationSource.Insert(0, new NotificationSourceBindableModel
        {
            Name = LocalizationResourceManager.Instance["All"] as string,
            NotificationsCount = _allNotifications.Count(),
            SelectCommand = SelectNotificationSourceCommand,
        });
    }

    private async Task LoadDevicesNotificationSourcesAsync()
    {
        var devicesNotificationSource = new ObservableCollection<NotificationSourceBindableModel>();

        var allDevices = await _devicesService.GetAllSupportedDevicesAsync();

        var notificationsFromDevices = _allNotifications.GroupBy(x => x.Device.FullDeviceId);

        foreach (var device in allDevices)
        {
            var notifications = notificationsFromDevices.FirstOrDefault(x => x.Key == device.FullDeviceId);

            var notificationSource = new NotificationSourceBindableModel
            {
                Id = device.FullDeviceId,
                Name = device.Name,
                NotificationsCount = notifications is null
                    ? 0
                    : notifications.Count(),
                SelectCommand = SelectNotificationSourceCommand,
            };

            devicesNotificationSource.Add(notificationSource);
        }

        _deviceeNotificationSource = new(devicesNotificationSource);
        _deviceeNotificationSource.Insert(0, new NotificationSourceBindableModel
        {
            Name = LocalizationResourceManager.Instance["All"] as string,
            NotificationsCount = _allNotifications.Count(),
            SelectCommand = SelectNotificationSourceCommand,
        });
    }

    private void FilterNotifications()
    {
        var notifications = Enumerable.Empty<NotificationGroupItemBindableModel>();

        switch (SelectedNotificationCategoryIndex)
        {
            case Constants.Filters.BY_ROOMS:

                notifications = string.IsNullOrEmpty(SelectedNotificationSource.Id)
                    ? _allNotifications
                    : _allNotifications.Where(x => x.Device.PositionId == SelectedNotificationSource.Id);

                // TO DO: hide room name in other way
                //foreach (var notification in notifications)
                //{
                //    notification.Device.RoomName = string.Empty;
                //}

                break;

            case Constants.Filters.BY_ACCESSORIES:

                notifications = string.IsNullOrEmpty(SelectedNotificationSource.Id)
                    ? _allNotifications
                    : _allNotifications.Where(x => x.Device.FullDeviceId == SelectedNotificationSource.Id);

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

    private void SetAndSelectNotificationSources()
    {
        NotificationsSources = SelectedNotificationCategoryIndex switch
        {
            Constants.Filters.BY_ROOMS => new(_roomsNotificationSource),
            Constants.Filters.BY_ACCESSORIES => new(_deviceeNotificationSource),
            _ => new(_roomsNotificationSource),
        };

        SelectNotificationSource(NotificationsSources.FirstOrDefault());
    }

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.RepeatAsync(LoadAllNotificationsAsync, executionTime);

            if (IsInternetConnected)
            {
                if (isDataLoaded)
                {
                    await Task.WhenAll(
                        LoadRoomsNotificationSourcesAsync(),
                        LoadDevicesNotificationSourcesAsync());

                    SetAndSelectNotificationSources();

                    FilterNotifications();

                    DataState = EPageState.Complete;
                }
                else
                {
                    DataState = EPageState.Empty;
                }
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
    }

    private async Task OnRefreshNotificationsCommandAsync()
    {
        if (!IsDataLoading)
        {
            await LoadNotificationsAndChangeStateAsync();

            IsNotificationsRefreshing = false;
        }
    }

    private async Task LoadNotificationsAndChangeStateAsync()
    {
        if (IsInternetConnected)
        {
            var isDataLoaded = await LoadAllNotificationsAsync();

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

    private async Task<bool> LoadAllNotificationsAsync()
    {
        bool isLoaded = false;

        if (IsInternetConnected)
        {
            List<NotificationGroupItemBindableModel> result = new();

            var devices = await _devicesService.GetAllDevicesAsync();

            var supportedDevices = await _devicesService.GetAllSupportedDevicesAsync();

            foreach (var device in devices)
            {
                var resourceIds = supportedDevices
                    .Where(x => x.IsReceiveNotifications && x.DeviceId == device.DeviceId && x.EditableResourceId is not null)
                    .Select(x => x.EditableResourceId)
                    .ToArray();

                if (!resourceIds.Any()) continue;

                var resultOfGettingNotifications = await _notificationsService.GetNotificationsForDeviceAsync(device.DeviceId, resourceIds);

                if (resultOfGettingNotifications.IsSuccess)
                {
                    result.AddRange(resultOfGettingNotifications.Result);
                }
            }
            
            result.Sort(Comparer<NotificationGroupItemBindableModel>.Create((item1, item2) => item2.LastActivityTime.CompareTo(item1.LastActivityTime)));

            _allNotifications = new(result);

            isLoaded = result.Any();
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
