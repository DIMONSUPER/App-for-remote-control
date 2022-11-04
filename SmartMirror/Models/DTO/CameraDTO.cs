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

    public bool IsShow { get; set; }

    public bool IsConnected { get; set; }

    public DateTime CreateTime { get; set; }

    public string VideoUrl { get; set; }

    #endregion
}

