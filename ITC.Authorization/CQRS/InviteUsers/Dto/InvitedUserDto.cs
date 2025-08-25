using ITC.Authorization.Enums;
using ITC.Domain.Dto;

namespace ITC.Authorization.CQRS.InviteUsers.Dto;

public class InvitedUserDto : EntityDtoBase
{
    public Guid? EmployeeId { get; set; }
    public Guid? UserId { get; set; }
    public string FirstName { get; set; } = String.Empty;

    public string LastName { get; set; } = String.Empty;

    public string? MiddleName { get; set; } = String.Empty;

    public Gender Gender { get; set; }

    public string Email { get; set; } = String.Empty;

    public string? Position { get; set; } = String.Empty;
    
    public string Role { get; set; } = String.Empty;
}