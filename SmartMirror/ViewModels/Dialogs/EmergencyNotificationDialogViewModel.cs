using SmartMirror.Helpers;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;
using SmartMirror.Resources.Strings;
using SmartMirror.Models.BindableModels;
using System.Windows.Input;
using System.Collections.ObjectModel;
using SmartMirror.Interfaces;
using System.ComponentModel;
using SmartMirror.Services.Notifications;

namespace SmartMirror.ViewModels.Dialogs;

public class EmergencyNotificationDialogViewModel : BaseDialogViewModel
{
    private readonly INotificationsService _notificationsService;

    public EmergencyNotificationDialogViewModel(
        IBlurService blurService,
        INotificationsService notificationsService,
        IKeyboardService keyboardService)
        : base(blurService, keyboardService)
    {
        _notificationsService = notificationsService;

        _notificationsService.AllNotificationsChanged += OnNotificationReceived;
    }

    #region -- Public properties --

    private ObservableCollection<NotificationGroupItemBindableModel> _notifications = new();
    public ObservableCollection<NotificationGroupItemBindableModel> Notifications
    {
        get => _notifications;
        set => SetProperty(ref _notifications, value);
    }

    private int _currentIndex = 1;
    public int CurrentIndex
    {
        get => _currentIndex;
        set => SetProperty(ref _currentIndex, value);
    }

    private int _itemIndex;
    public int ItemIndex
    {
        get => _itemIndex;
        set => SetProperty(ref _itemIndex, value);
    }

    private NotificationGroupItemBindableModel _currentItem;
    public NotificationGroupItemBindableModel CurrentItem
    {
        get => _currentItem;
        set => SetProperty(ref _currentItem, value);
    }

    private ICommand _nextCommand;
    public ICommand NextCommand => _nextCommand ??= SingleExecutionCommand.FromFunc(OnNextCommandAsync);

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        if (args.PropertyName == nameof(ItemIndex))
        {
            CurrentIndex = ItemIndex + 1;
        }
    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        base.OnDialogOpened(parameters);

        if (parameters.TryGetValue(Constants.DialogsParameterKeys.NOTIFICATION, out NotificationGroupItemBindableModel notification))
        {
            Notifications.Add(notification);
        }
    }

    public override void OnDialogClosed()
    {
        base.OnDialogClosed();

        _notificationsService.AllNotificationsChanged -= OnNotificationReceived;
    }

    #endregion

    #region -- Private helpers --

    private Task OnNextCommandAsync()
    {
        if (Notifications.Count > CurrentIndex)
        {
            CurrentItem = null;
            CurrentItem = Notifications[ItemIndex + 1];

            CurrentIndex++;
        }

        return Task.CompletedTask;
    }

    private void OnNotificationReceived(object sender, NotificationGroupItemBindableModel notification)
    {
        if (notification is not null && notification.IsEmergencyNotification)
        {
            Notifications.Add(notification);
        }
    }

    #endregion
}

