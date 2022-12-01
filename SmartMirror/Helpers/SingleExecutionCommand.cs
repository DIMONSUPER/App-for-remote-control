using System.Windows.Input;

namespace SmartMirror.Helpers;

public class SingleExecutionCommand : ICommand
{
    public static bool IsNavigating { get; set; }

    private static readonly object _locker = new();

    private Func<object, Task> _func;
    private bool _isExecuting;
    private bool _useNavigation;
    private int _delayMillisec;

    public SingleExecutionCommand()
    {
    }

    private SingleExecutionCommand(object onContinueCommand)
    {
    }

    public static SingleExecutionCommand FromFunc(Func<Task> func, bool useNavigation = false, int delayMillisec = 400)
    {
        return new SingleExecutionCommand
        {
            _func = (obj) => func(),
            _useNavigation = useNavigation,
            _delayMillisec = delayMillisec
        };
    }

    public static SingleExecutionCommand FromFunc(Func<object, Task> func, bool useNavigation = false, int delayMillisec = 400)
    {
        return new SingleExecutionCommand
        {
            _func = func,
            _useNavigation = useNavigation,
            _delayMillisec = delayMillisec
        };
    }

    public static SingleExecutionCommand FromFunc<T>(Func<T, Task> func, bool useNavigation = false, int delayMillisec = 400)
    {
        return new SingleExecutionCommand
        {
            _func = (object obj) =>
            {
                var objT = default(T);
                objT = (T)obj;

                return func(objT);
            },
            _useNavigation = useNavigation,
            _delayMillisec = delayMillisec
        };
    }

    internal static ICommand FromFunc(ICommand goBackCommand)
    {
        return goBackCommand;
    }

    #region -- ICommand implementation --

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public async void Execute(object parameter)
    {
        lock (_locker)
        {
            if (_isExecuting || (IsNavigating && _useNavigation)) return;

            IsNavigating = _useNavigation;

            _isExecuting = true;
        }

        await _func(parameter);

        if (_delayMillisec > 0)
        {
            await Task.Delay(_delayMillisec);
        }

        _isExecuting = false;

        if (_useNavigation)
        {
            IsNavigating = false;
        }
    }

    #endregion
}
