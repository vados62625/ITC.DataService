namespace ITC.Authorization.ServiceBus.Organization;

public class AddEmployeeFromInvitedUserMqResult
{
    public Guid EmployeeId { get; set; }
    public string InviteToken { get; set; }
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid BrokerId { get; set; }
    public string Phone { get; set; }
}