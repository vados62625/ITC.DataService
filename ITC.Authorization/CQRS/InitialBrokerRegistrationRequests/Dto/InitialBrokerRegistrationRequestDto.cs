using ITC.Domain.Dto;

namespace ITC.Authorization.CQRS.InitialBrokerRegistrationRequests.Dto;

public class InitialBrokerRegistrationRequestDto : EntityDtoBase
{
    public Guid? BrokerId { get; set; }
    public Guid? CompanyId { get; set; }
    public bool IsCreated => BrokerId.HasValue && CompanyId.HasValue;
}