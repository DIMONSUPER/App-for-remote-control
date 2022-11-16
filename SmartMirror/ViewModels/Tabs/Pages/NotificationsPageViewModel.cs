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

        NotificationsSources = new ObservableCollection<NotificationSourceBindableModel>()
        {
            new NotificationSourceBindableModel
            {
                Name = "All",
                NotificationsCount = 123,
                SelectCommand = SelectNotificationSourceCommand,
            },
            new NotificationSourceBindableModel
            {
                Name = "Dining Room",
                NotificationsCount = 23,
                SelectCommand = SelectNotificationSourceCommand,
            },
            new NotificationSourceBindableModel
            {
                Name = "Front Door",
                NotificationsCount = 54,
                SelectCommand = SelectNotificationSourceCommand,
            },
            new NotificationSourceBindableModel
            {
                Name = "Garage",
                NotificationsCount = 12,
                SelectCommand = SelectNotificationSourceCommand,
            },
            new NotificationSourceBindableModel
            {
                Name = "Half Bath",
                NotificationsCount = 3,
                SelectCommand = SelectNotificationSourceCommand,
            },
            new NotificationSourceBindableModel
            {
                Name = "Living Room",
                NotificationsCount = 30,
                SelectCommand = SelectNotificationSourceCommand,
            },
            new NotificationSourceBindableModel
            {
                Name = "Garage",
                NotificationsCount = 12,
                SelectCommand = SelectNotificationSourceCommand,
            },
            new NotificationSourceBindableModel
            {
                Name = "Half Bath",
                NotificationsCount = 3,
                SelectCommand = SelectNotificationSourceCommand,
            },
            new NotificationSourceBindableModel
            {
                Name = "Living Room",
                NotificationsCount = 30,
                SelectCommand = SelectNotificationSourceCommand,
            },
        };

        SelectNotificationSource(NotificationsSources.FirstOrDefault());
    }

    #region -- Public properties --

    private ObservableCollection<string> _notificationsFilters = new() { "Rooms", "Accessories" };
    public ObservableCollection<string> NotificationsFilters
    {
        get => _notificationsFilters;
        set => SetProperty(ref _notificationsFilters, value);
    }

    private int _selectedNotificationFilterIndex;
    public int SelectedNotificationFilterIndex
    {
        get => _selectedNotificationFilterIndex;
        set => SetProperty(ref _selectedNotificationFilterIndex, value);
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

    private ICommand _selectNotificationSourceCommand;
    public ICommand SelectNotificationSourceCommand => _selectNotificationSourceCommand ??= 
        SingleExecutionCommand.FromFunc<NotificationSourceBindableModel>(OnSelectNotificationSourceCommandAsync, delayMillisec: 0);

    private ICommand _selectedNotificationFilterChangedCommand;
    public ICommand SelectedNotificationFilterChangedCommand => _selectedNotificationFilterChangedCommand ??=
        SingleExecutionCommand.FromFunc(OnSelectedNotificationFilterChangedCommandAsync);

    private Task OnSelectedNotificationFilterChangedCommandAsync()
    {
        return Task.CompletedTask;
    }

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

    protected override void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        base.OnConnectivityChanged(sender, e);

        if (e.NetworkAccess != NetworkAccess.Internet)
        {
            IsNotificationsRefreshing = false;
        }
    }

    #endregion

    #region -- Private helpers --

    private async void OnNotificationReceived(object sender, NotificationGroupItemBindableModel notification)
    {
        await LoadNotificationsAndChangeStateAsync();
    }

    private void OnAllRoomsChanged(object sender, EventArgs e)
    {
    }

    private async void OnAllDevicesChanged(object sender, EventArgs e)
    {
        DataState = EPageState.LoadingSkeleton;

        var devices = await _devicesService.GetAllSupportedDevicesAsync();

        if (devices.Any())
        {
            await LoadNotificationsAndChangeStateAsync();
        }
        else
        {
            DataState = EPageState.Empty;
        }
    }

    private Task OnSelectNotificationSourceCommandAsync(NotificationSourceBindableModel notificationSource)
    {
        SelectNotificationSource(notificationSource);

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

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.RepeatAsync(LoadNotificationsAsync, executionTime);

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
            var isDataLoaded = await LoadNotificationsAsync();

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

    private async Task<bool> LoadNotificationsAsync()
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

            _ = Task.Run(() => Notifications = new(GetNotificationGroups(result)));

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
