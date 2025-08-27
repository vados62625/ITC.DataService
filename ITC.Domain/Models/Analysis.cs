using ITC.Domain.Enums;

namespace ITC.Domain.Models;

public class Analysis : EntityBase
{
    public Guid EngineId { get; set; }
    
    public double OuterRingDefect { get; set; }
        
    public double InnerRingDefect { get; set; }
        
    public double RollingElementsDefect { get; set; }
        
    public double CageDefect { get; set; }
        
    public double Unbalance { get; set; }
        
    public double Misalignment { get; set; }
    public DateTime DateTime { get; set; }
}