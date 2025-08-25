using AutoMapper;
using ITC.Authorization.CQRS.PasswordResetUsers.Dto;
using ITC.Authorization.Storage;
using ITC.CQRS.Base.Get;

namespace ITC.Authorization.CQRS.PasswordResetUsers;

public class Get : GetCollectionQueryBase
{
    public Guid? UserId { get; set; }
    public string? PasswordResetToken { get; set; }

    public class Handler : UserGetCollectionQueryHandlerBase<ServiceDbContext, Get, PasswordResetUser, PasswordResetUserDto>
    {
        public Handler(IMapper mapper, ServiceDbContext db) : base(mapper, db)
        {
        }

        protected override ValueTask<IQueryable<PasswordResetUser>> PreRequestAction(UserApiGetCollectionRequestWrapper<Get, PasswordResetUserDto> request, IQueryable<PasswordResetUser> query,
            CancellationToken cancellationToken)
        {
            if (request.Request.UserId.HasValue)
                query = query.Where(c => c.UserId == request.Request.UserId);
            
            if (!string.IsNullOrEmpty(request.Request.PasswordResetToken))
                query = query.Where(c => c.PasswordResetToken == request.Request.PasswordResetToken);
            
            return ValueTask.FromResult(query);
        }
    }
}