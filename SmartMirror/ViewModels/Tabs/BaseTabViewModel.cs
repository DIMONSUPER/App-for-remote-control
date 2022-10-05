namespace SmartMirror.ViewModels.Tabs;

public class BaseTabViewModel : BindableBase, IInitialize, IInitializeAsync, IPageLifecycleAware
{
    public BaseTabViewModel()
    {
    }

    #region -- Public properties --

    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    #endregion

    #region -- IInitialize implementation --

    public virtual void Initialize(INavigationParameters parameters)
    {
    }

    #endregion

    #region -- IInitializeAsync implementation --

    public virtual Task InitializeAsync(INavigationParameters parameters)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region -- IPageLifecycleAware implementation --

    public virtual void OnAppearing()
    {
    }

    public virtual void OnDisappearing()
    {
    }

    #endregion
}

