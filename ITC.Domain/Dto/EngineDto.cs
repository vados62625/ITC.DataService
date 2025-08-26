using ITC.Domain.Enums;
using ITC.Domain.Models;

namespace ITC.Domain.Dto;

public class EngineDto : EntityDtoBase
{
    public bool IsLastAnalyseHasDefect { get; set; }
    public EngineStatus Status { get; set; }
    public List<DefectDto> Defects { get; set; } = new();
    public DateTime LastAnalyseDate { get; set; }
}