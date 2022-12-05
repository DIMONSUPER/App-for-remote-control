using System;

namespace SmartMirror.Models.Aqara;

public class LinkageConditionsAqaraModel
{
    public List<ConditionAqaraModel> Condition { get; set; }

    public string Relation { get; set; }
}