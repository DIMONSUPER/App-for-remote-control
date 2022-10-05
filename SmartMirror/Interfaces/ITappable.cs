using System.Windows.Input;

namespace SmartMirror.Interfaces
{
    public interface ITappable
    {
        ICommand? TapCommand { get; set; }
    }
}
