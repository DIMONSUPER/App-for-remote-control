using SmartMirror.Interfaces;
using SQLite;

namespace SmartMirror.Models.DTO;

[Table(nameof(CameraDTO))]
public class CameraDTO : IDTO
{
    #region -- IDTO implementation --

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    #endregion

    #region -- Public properties --

    public string Name { get; set; }

    public string IpAddress { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public bool IsShown { get; set; }

    public bool IsConnected { get; set; }

    public bool IsReceiveNotifications { get; set; }

    public bool IsEmergencyNotification { get; set; }

    public DateTime CreateTime { get; set; }

    public string VideoUrl { get; set; }

    public int SubType { get; set; }

    public int Channel { get; set; }

    #endregion
}

