using System;

namespace SmartMirror.Models.Aqara;

public class ConditionAqaraModel
{
    public string TriggerDefinitionId { get; set; }

    public string TriggerName { get; set; }

    public string SubjectId { get; set; }

    public string Model { get; set; }

    public string BeginTime { get; set; }

    public string EndTime { get; set; }

    public List<ParamAqaraModel> Params { get; set; }
}

