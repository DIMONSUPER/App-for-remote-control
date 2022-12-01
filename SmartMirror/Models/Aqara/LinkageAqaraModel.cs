using SmartMirror.Enums;

namespace SmartMirror.Models.Aqara;

public class LinkageAqaraModel
{
    public string LinkageId { get; set; }

    public string Model { get; set; }

    public string Name { get; set; }

    public ELocalizeAqara Localize { get; set; }

    public bool Enable { get; set; }
}