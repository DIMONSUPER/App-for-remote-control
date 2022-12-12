namespace SmartMirror.Models.Aqara;

public class LinkageActionAqaraModel
{
    public string SubjectId { get; set; }

    public string ActionName { get; set; }

    public string Model { get; set; }

    public string DelayTime { get; set; }

    public string DelayTimeUnit { get; set; }

    public string ActionDefinitionId { get; set; }

    public List<ParamAqaraModel> Params { get; set; }
}

