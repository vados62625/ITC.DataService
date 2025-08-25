namespace ITC.CQRS.Base.Suggest;

public class SuggestResultDto 
{
    public Guid Id { get; set; }
    public required string Value { get; set; }
}