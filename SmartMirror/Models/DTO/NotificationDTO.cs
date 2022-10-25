using SmartMirror.Interfaces;
using SQLite;

namespace SmartMirror.Models.DTO;

[Table(nameof(NotificationDTO))]
public class NotificationDTO : IDTO
{
    #region -- IDTO implementation --

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    #endregion

    #region -- Public properties --

    public string Name { get; set; }

    public string Type { get; set; }

    public string Status { get; set; }

    public DateTime LastActivityTime { get; set; }

    #endregion
}

