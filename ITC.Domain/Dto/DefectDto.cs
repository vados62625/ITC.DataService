using ITC.Domain.Enums;

namespace ITC.Domain.Dto;

public class DefectDto : EntityDtoBase
{
    public Guid EngineId { get; set; }
    public DefectType Type { get; set; }
    public string? Name { get; set; }
    public List<DefectHistoryDto> History { get; set; } = new();
}