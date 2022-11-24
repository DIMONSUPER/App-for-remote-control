using System.Windows.Input;
using SmartMirror.Helpers;
using System.ComponentModel;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;

namespace SmartMirror.ViewModels.Dialogs;

public class BaseDialogViewModel : BindableBase, IDialogAware
{
    protected const int FOCUS_DELAY = 350;

    public BaseDialogViewModel(
        IBlurService blurService,
        IKeyboardService keyboardService)
    {
        BlurService = blurService;
        KeyboardService = keyboardService;

        var blurColor = Color.FromArgb("#80030303");
        BlurService.BlurPopupBackground(blurColor);

        if (IsKeyboardSensitive)
        {
            SubscribeToKeyboardChanges();
        }
    }

    #region -- Public properties --

    private double _keyboardMargin;
    public double KeyboardMargin
    {
        get => _keyboardMargin;
        set => SetProperty(ref _keyboardMargin, value);
    }

    private double _viewHeight;
    public double ViewHeight
    {
        get => _viewHeight;
        set => SetProperty(ref _viewHeight, value);
    }

    private bool _isKeyboardSensitive = true;
    public bool IsKeyboardSensitive
    {
        get => _isKeyboardSensitive;
        set => SetProperty(ref _isKeyboardSensitive, value);
    }

    #endregion

    #region -- Protected properties --

    protected bool IsInternetConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    protected IBlurService BlurService { get; }

    protected IKeyboardService KeyboardService { get; }

    private ICommand _closeCommand;
    public ICommand CloseCommand => _closeCommand ??= SingleExecutionCommand.FromFunc(OnCloseCommandAsync);

    #endregion

    #region -- IDialogAware implementation --

    public DialogCloseEvent RequestClose { get; set; }

    public virtual bool CanCloseDialog()
    {
        return true;
    }

    public virtual void OnDialogClosed()
    {
        BlurService.UnblurPopupBackground();

        if (IsKeyboardSensitive)
        {
            UnsubscribeFromKeyboardChanges();
        }
    }

    public virtual void OnDialogOpened(IDialogParameters parameters)
    {
    }

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        if (args.PropertyName is nameof(IsKeyboardSensitive))
        {
            if (IsKeyboardSensitive)
            {
                SubscribeToKeyboardChanges();
            }
            else
            {
                UnsubscribeFromKeyboardChanges();
            }
        }
    }

    #endregion

    #region -- Public helpers --

    public virtual Task OnCloseCommandAsync()
    {
        RequestClose.Invoke();

        return Task.CompletedTask;
    }

    #endregion

    #region -- Private helpers --

    private void SubscribeToKeyboardChanges()
    {
        KeyboardMargin = CalculateTranslationForView();
        KeyboardService.KeyboardHeightChanged += OnKeyboardHeightChanged;
    }

    private void UnsubscribeFromKeyboardChanges()
    {
        KeyboardMargin = 0;
        KeyboardService.KeyboardHeightChanged -= OnKeyboardHeightChanged;
    }

    private double CalculateTranslationForView()
    {
        double neededTranslation = 0;

        var allHeight = App.Current.MainPage.Height;

        var availableHeight = (allHeight - ViewHeight) / 2 - 20;

        var insect = (allHeight + ViewHeight) / 2 + KeyboardService.KeyboardHeight;

        if (insect > allHeight)
        {
            neededTranslation = insect - allHeight;
        }

        if (neededTranslation > availableHeight)
        {
            neededTranslation = availableHeight;
        }

        return neededTranslation * 2;
    }

    private void OnKeyboardHeightChanged(object sender, EventArgs e)
    {
        KeyboardMargin = CalculateTranslationForView();
    }

    #endregion
}