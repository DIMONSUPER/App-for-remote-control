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

    public string DeviceId { get; set; }

    public string PositionId { get; set; }

    public string Name { get; set; }

    public EDeviceStatus Status { get; set; }

    public EDeviceType DeviceType { get; set; }

    public string IconSource { get; set; }

    public string RoomName { get; set; }

    public string AdditionalInfo { get; set; }

    public string ParentDid { get; set; }

    public DateTime CreateTime { get; set; }

    public string TimeZone { get; set; }

    public string Model { get; set; }

    public DateTime UpdateTime { get; set; }

    public int ModelType { get; set; }

    public int State { get; set; }

    public string EditableResourceId { get; set; }

    public EUnitMeasure UnitMeasure { get; set; }

    public bool IsShownInRooms { get; set; }

    public bool IsReceiveNotifications { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsEmergencyNotification { get; set; }

    #endregion
}

