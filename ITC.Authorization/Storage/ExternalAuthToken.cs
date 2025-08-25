using ITC.Domain.Models;

namespace ITC.Authorization.Storage;

public class ExternalAuthToken : EntityBase
{
    public Guid Token { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset ValidTo { get; set; }
    public string JwtRefresh { get; set; } = string.Empty;
}