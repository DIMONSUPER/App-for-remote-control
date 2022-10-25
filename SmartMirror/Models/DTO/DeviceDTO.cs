using SmartMirror.Enums;
using SmartMirror.Interfaces;
using SQLite;

namespace SmartMirror.Models.DTO;

[Table(nameof(DeviceDTO))]
public class DeviceDTO : IDTO
{
    #region -- IDTO implementation --

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    #endregion

    #region -- Public properties --

    public string DeviceId;

    public string PositionId;

    public string Name;

    public EDeviceStatus Status;

    public EDeviceType DeviceType;

    public string IconSource;

    public string RoomName;

    public string AdditionalInfo;

    public string ParentDid;

    public DateTime CreateTime;

    public string TimeZone;

    public string Model;

    public DateTime UpdateTime;

    public int ModelType;

    public int State;

    public string EditableResourceId;

    #endregion
}

