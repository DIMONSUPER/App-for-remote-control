using SmartMirror.Interfaces;
using SQLite;

namespace SmartMirror.Models.DTO;

[Table(nameof(AttributeAqaraDTO))]
public class AttributeAqaraDTO : IDTO
{
    #region -- IDTO implementation --

    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; }

    #endregion

    #region -- Public properties --

    public string Enums { get; set; }

    public string ResourceId { get; set; }

    public long? MinValue { get; set; }

    public int? Unit { get; set; }

    public int? Access { get; set; }

    public long? MaxValue { get; set; }

    public string DefaultValue { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Model { get; set; }

    #endregion
}

