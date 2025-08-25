namespace ITC.Authorization.ServiceBus.Organization;

public class AddEmployeeFromInvitedUserMq
{
    public required string InviteToken { get; set; }
    public required Guid UserId { get; set; }
    public string Phone { get; set; }
}