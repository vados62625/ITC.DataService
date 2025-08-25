using System.ComponentModel.DataAnnotations;

namespace ITC.CQRS.Base.Delete;

public abstract class DeleteEntityCommandBase
{
    [Required]
    public Guid Id { get; set; }
}