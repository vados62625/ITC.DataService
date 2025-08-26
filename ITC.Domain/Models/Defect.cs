using ITC.Domain.Enums;

namespace ITC.Domain.Models;

public class Defect : EntityBase
{
    public Guid EngineId { get; set; }
    public DefectType Type { get; set; }
    public string? Name { get; set; }
}