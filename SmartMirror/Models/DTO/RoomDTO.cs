using SmartMirror.Interfaces;
using SQLite;

namespace SmartMirror.Models.DTO;

[Table(nameof(RoomDTO))]
public class RoomDTO : IDTO
{
    #region -- IDTO implementation --

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    #endregion

    #region -- Public properties --

    public string Name;

    public DateTime CreateTime;

    public string Description;

    public int DevicesCount;

    #endregion
}
