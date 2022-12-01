namespace SmartMirror.Models.BindableModels
{
    public class ScenarioActionBindableModel : BindableBase
    {
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _actionDefinitionId;
        public string ActionDefinitionId
        {
            get => _actionDefinitionId;
            set => SetProperty(ref _actionDefinitionId, value);
        }

        private object _model;
        public object Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private string _delayTime;
        public string DelayTime
        {
            get => _delayTime;
            set => SetProperty(ref _delayTime, value);
        }

        private string _delayTimeUnit;
        public string DelayTimeUnit
        {
            get => _delayTimeUnit;
            set => SetProperty(ref _delayTimeUnit, value);
        }

        private string _subjectId;
        public string SubjectId
        {
            get => _subjectId;
            set => SetProperty(ref _subjectId, value);
        }

        private DeviceBindableModel _device;
        public DeviceBindableModel Device
        {
            get => _device;
            set => SetProperty(ref _device, value);
        }

        private string _actionName;
        public string ActionName
        {
            get => _actionName;
            set => SetProperty(ref _actionName, value);
        }

        private DateTime _actionTime;
        public DateTime ActionTime
        {
            get => _actionTime;
            set => SetProperty(ref _actionTime, value);
        }
    }
}
