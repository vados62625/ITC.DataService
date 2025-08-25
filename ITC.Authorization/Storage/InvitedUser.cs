using ITC.Authorization.Enums;
using ITC.Domain.Models;

namespace ITC.Authorization.Storage;

public class InvitedUser : EntityBase
{
    public Guid? EmployeeId { get; set; }
    public Guid? UserId { get; set; }

    public string FirstName { get; set; } = String.Empty;

    public string LastName { get; set; } = String.Empty;

    public string? MiddleName { get; set; }

    public Gender Gender { get; set; }

    public string Email { get; set; } = String.Empty;

    public string? Position { get; set; }


    public string Role { get; set; } = String.Empty;


    public string InviteToken { get; set; } = String.Empty;
}
