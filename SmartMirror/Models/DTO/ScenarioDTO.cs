using SmartMirror.Interfaces;
using SQLite;

namespace SmartMirror.Models.DTO;

[Table(nameof(ScenarioDTO))]
public class ScenarioDTO : IDTO
{
    #region -- IDTO implementation --

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    #endregion

    #region -- Public properties --

    public string SceneId { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public bool IsShownInScenarios { get; set; }

    public bool IsReceiveNotifications { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsEmergencyNotification { get; set; }

    public DateTime ActivationTime { get; set; }

    #endregion
}

