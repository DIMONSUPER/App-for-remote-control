using System.Windows.Input;

namespace SmartMirror.Helpers;

public class SingleExecutionCommand : ICommand
{
    public static bool IsNavigating;

    private Func<object, Task> _func;
    private bool _isExecuting;
    private int _delayMillisec;

    private SingleExecutionCommand(object onContinueCommand)
    {

    }

    public SingleExecutionCommand()
    {
    }

    public static SingleExecutionCommand FromFunc(Func<Task> func, int delayMillisec = 400)
    {
        var ret = new SingleExecutionCommand
        {
            _func = (obj) => func(),
            _delayMillisec = delayMillisec
        };
        return ret;
    }

    public static SingleExecutionCommand FromFunc(Func<object, Task> func, int delayMillisec = 400)
    {
        var ret = new SingleExecutionCommand
        {
            _func = func,
            _delayMillisec = delayMillisec
        };
        return ret;
    }

    public static SingleExecutionCommand FromFunc<T>(Func<T, Task> func, int delayMillisec = 400)
    {
        var ret = new SingleExecutionCommand
        {
            _func = (object obj) =>
            {
                var objT = default(T);
                objT = (T)obj;
                return func(objT);
            },
            _delayMillisec = delayMillisec
        };
        return ret;
    }

    internal static ICommand FromFunc(ICommand goBackCommand)
    {
        return goBackCommand;
    }

    #region -- ICommand implementation --

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        //TODO: improve it
        return true;
    }

    public async void Execute(object parameter)
    {
        if (_isExecuting || IsNavigating) return;

        _isExecuting = true;

        await _func(parameter);
        if (_delayMillisec > 0)
        {
            await Task.Delay(_delayMillisec);
        }

        _isExecuting = false;
    }

    #endregion
}
