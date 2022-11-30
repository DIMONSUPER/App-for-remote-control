using SmartMirror.Helpers;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;
using SmartMirror.Resources.Strings;
using SmartMirror.Models.BindableModels;
using System.Windows.Input;
using System.Collections.ObjectModel;
using SmartMirror.Interfaces;
using System.ComponentModel;

namespace SmartMirror.ViewModels.Dialogs;

public class EmergencyNotificationDialogViewModel : BaseDialogViewModel
{
    public EmergencyNotificationDialogViewModel(
        IBlurService blurService,
        IKeyboardService keyboardService)
        : base(blurService, keyboardService)
    {
    }

    #region -- Public properties --

    private ObservableCollection<INotificationGroupItemModel> _notifications;
    public ObservableCollection<INotificationGroupItemModel> Notifications
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

    private INotificationGroupItemModel _currentItem;
    public INotificationGroupItemModel CurrentItem
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

        if (parameters.TryGetValue(Constants.DialogsParameterKeys.ACCESSORY, out ObservableCollection<INotificationGroupItemModel> notifications))
        {
            Notifications = new(notifications.Skip(1).Take(10));
        }
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

    #endregion
}

