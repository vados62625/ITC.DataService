namespace ITC.Domain.Dto;

public class DiagramItemDto
{
    public double Value { get; set; }
    public PhaseType Type { get; set; }
    public DateTime Time { get; set; }

    public DiagramItemDto()
    {
    }

    public DiagramItemDto(double value, PhaseType type, DateTime time)
    {
        Value = value;
        Type = type;
        Time = time;
    }

    public override string ToString()
    {
        return $"Value: {Value}, Type: {Type}, Time: {Time:yyyy-MM-dd HH:mm:ss}";
    }
}

public enum PhaseType
{
    R,
    S,
    T
}