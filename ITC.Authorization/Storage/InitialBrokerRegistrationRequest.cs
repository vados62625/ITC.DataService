using ITC.Domain.Models;

namespace ITC.Authorization.Storage;

public class InitialBrokerRegistrationRequest : EntityBase
{
    public required Guid UserId { get; set; }
    public Guid? BrokerId { get; set; }
    public Guid? CompanyId { get; set; }
}