using AutoMapper;
using ITC.Authorization.CQRS.UserRolesCaches.Dto;
using ITC.Authorization.Storage;
using ITC.CQRS.Base.Get;

namespace ITC.Authorization.CQRS.UserRolesCaches;

public class Get : GetCollectionQueryBase
{
    public Guid? UserId { get; set; }
    public string[] Roles { get; set; }

    public class Handler : UserGetCollectionQueryHandlerBase<ServiceDbContext, Get, UserRolesCache, UserRolesCacheDto>
    {
        public Handler(IMapper mapper, ServiceDbContext db) : base(mapper, db)
        {

        }
    }
}