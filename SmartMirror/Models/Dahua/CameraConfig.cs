namespace SmartMirror.Models.Dahua;

public class CameraConfig
{
    public List<FormatConfig> ExtraFormat { get; set; }
    public List<FormatConfig> MainFormat { get; set; }
    public List<FormatConfig> SnapFormat { get; set; }
}

public class FormatConfig
{
    public AudioConfig Audio { get; set; }
    public bool AudioEnable { get; set; }
    public VideoConfig Video { get; set; }
    public bool VideoEnable { get; set; }
}

public class VideoConfig
{
    public int BitRate { get; set; }
    public string BitRateControl { get; set; }
    public string Compression { get; set; }
    public int FPS { get; set; }
    public int GOP { get; set; }
    public int Height { get; set; }
    public string Pack { get; set; }
    public string Profile { get; set; }
    public int Quality { get; set; }
    public int QualityRange { get; set; }
    public int SVCTLayer { get; set; }
    public int Width { get; set; }
}

public class AudioConfig
{
    public string AudioSource { get; set; }
    public int BitRate { get; set; }
    public string Compression { get; set; }
    public int Depth { get; set; }
    public int Frequency { get; set; }
    public int Mode { get; set; }
    public string Pack { get; set; }
    public int PacketPeriod { get; set; }
}

