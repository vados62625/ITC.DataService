namespace ITC.Domain.Models
{
    public interface ITimeStampedModel
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }
    }
}