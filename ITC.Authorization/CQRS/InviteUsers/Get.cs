using AutoMapper;
using ITC.Authorization.CQRS.InviteUsers.Dto;
using ITC.Authorization.Storage;
using ITC.CQRS.Base.Get;

namespace ITC.Authorization.CQRS.InviteUsers;

public class Get : GetCollectionQueryBase
{
    public string InviteToken { get; set; } = string.Empty;

    public class Handler : UserGetCollectionQueryHandlerBase<ServiceDbContext, Get, InvitedUser, InvitedUserDto>
    {
        public Handler(IMapper mapper, ServiceDbContext db) : base(mapper, db)
        {
        }

        protected override ValueTask<IQueryable<InvitedUser>> PreRequestAction(UserApiGetCollectionRequestWrapper<Get, InvitedUserDto> request, IQueryable<InvitedUser> query,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Request.InviteToken))
            {
                query = query.Where(c => c.InviteToken == request.Request.InviteToken);
            }

            return ValueTask.FromResult(query);
        }
    }
}