using ITC.Domain.Models;

namespace ITC.Authorization.Storage;

public class UserRolesCache : EntityBase
{
    public Guid UserId { get; set; }
    public string[] Roles { get; set; }
}