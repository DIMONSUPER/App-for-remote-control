using SmartMirror.Enums;
using SmartMirror.Interfaces;
using SQLite;

namespace SmartMirror.Models.DTO;

[Table(nameof(AutomationDTO))]
public class AutomationDTO : IDTO
{
    #region -- IDTO implementation --

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    #endregion

    #region -- Public properties --

    public string LinkageId { get; set; }

    public string Model { get; set; }

    public string Name { get; set; }

    public ELocalizeAqara Localize { get; set; }

    public bool Enable { get; set; }

    public bool IsShownInAutomations { get; set; }

    public bool IsReceiveNotifications { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsEmergencyNotification { get; set; }

    #endregion
}
