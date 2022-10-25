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

    public string SceneId;

    public string Name;

    public bool IsFavorite;

    public DateTime ActivationTime;

    #endregion
}

