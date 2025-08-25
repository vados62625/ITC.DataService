using ITC.Domain.Enums;

namespace ITC.Domain.Models;

public class Engine : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public EngineStatus EngineStatus { get; set; }
}