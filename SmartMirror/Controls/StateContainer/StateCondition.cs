using System;
namespace SmartMirror.Controls.StateContainer
{
    [ContentProperty("Content")]
    public class StateCondition : View
    {
        public object State { get; set; }
        public View Content { get; set; }
    }
}

