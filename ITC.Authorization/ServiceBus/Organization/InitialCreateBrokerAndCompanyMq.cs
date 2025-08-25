namespace ITC.Authorization.ServiceBus.Organization;

public class InitialCreateBrokerAndCompanyMq
{
    public required Guid RegistrationRequestId { get; set; }
    public required string CompanyName { get; set; }
    public required Guid UserId { get; set; }
    public required string EmployeeFirstName { get; set; }
    public required string EmployeeMiddleName { get; set; }
    public required string EmployeeLastName { get; set; }
    public required string EmployeePhone { get; set; }
    public required string EmployeePosition { get; set; }
    public required string Email { get; set; }
}