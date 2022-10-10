namespace SmartMirror.Models.BindableModels
{
    public class ScenarioActionBindableModel : BindableBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _type;
        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _action;
        public string Action
        {
            get => _action;
            set => SetProperty(ref _action, value);
        }

        private DateTime _actionTime;
        public DateTime ActionTime
        {
            get => _actionTime;
            set => SetProperty(ref _actionTime, value);
        }
    }
}
