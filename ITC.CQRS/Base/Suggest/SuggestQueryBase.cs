using System.ComponentModel.DataAnnotations;

namespace ITC.CQRS.Base.Suggest;

public interface ISuggestRequestBase
{
    Guid? Id { get; set; }
    int MaxItems { get; set; }
    string? Suggest { get; set; }
}

public abstract class SuggestRequestBase : ISuggestRequestBase
{
    public Guid? Id { get; set; }

    [Required]
    public int MaxItems { get; set; }
    public string? Suggest { get; set; }
}