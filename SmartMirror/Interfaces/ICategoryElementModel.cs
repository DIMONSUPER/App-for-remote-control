using SmartMirror.Enums;

namespace SmartMirror.Interfaces
{
    public interface ICategoryElementModel
    {
        string ImageSource { get; set;  }

        ECategoryType Type { get; set;  }
    }
}
