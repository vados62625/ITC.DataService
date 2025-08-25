using ITC.Domain.Models;

namespace ITC.Authorization.Storage;

public class PasswordResetUser : EntityBase
{
    public required Guid UserId { get; set; }
    
    public required string PasswordResetToken { get; set; }
}