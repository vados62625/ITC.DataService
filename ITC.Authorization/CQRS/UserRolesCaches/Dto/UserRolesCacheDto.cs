using ITC.Domain.Dto;

namespace ITC.Authorization.CQRS.UserRolesCaches.Dto;

public class UserRolesCacheDto : EntityDtoBase
{
    public Guid? UserId { get; set; }
    public string[] Roles { get; set; }
}