namespace ITC.Authorization.CQRS.InviteUsers.Dto;

public class CreatedInvitedUser
{
    public string InviteToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}