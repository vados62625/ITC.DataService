using ITC.Domain.Enums;

namespace ITC.Domain.Models;

public class Engine : EntityBase
{
    public string? Name { get; set; }
    public EngineStatus EngineStatus { get; set; }
    public List<Analysis> Analyses { get; set; } = new();
}