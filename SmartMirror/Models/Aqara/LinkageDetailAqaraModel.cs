namespace SmartMirror.Models.Aqara
{
    public class LinkageDetailAqaraModel
    {
        public string LinkageId { get; set; }

        public string Name { get; set; }

        public bool Enable { get; set; }

        public LinkageConditionsAqaraModel Conditions { get; set; }

        public LinkageActionsAqaraModel Actions { get; set; }
    }
}

