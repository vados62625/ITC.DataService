namespace ITC.Authorization.ServiceBus.Organization;

public class UpdateUserRolesMq
{
    public Guid UserId { get; set; }
    public string[] Roles { get; set; }
}