using ITC.Domain.Dto;

namespace ITC.Authorization.CQRS.PasswordResetUsers.Dto;

public class PasswordResetUserDto : EntityDtoBase
{
    public required Guid UserId { get; set; }
    
    public required string PasswordResetToken { get; set; }
}