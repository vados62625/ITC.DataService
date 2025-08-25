namespace ITC.Domain.Models;

public abstract class EntityBase : ITimeStampedModel, IEntity
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Timestamp]
    public uint Version { get; set; }

    public long LockToken { get; set; }

    public long Timestamp { get; set; }
}