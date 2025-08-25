using AutoMapper;
using ITC.Authorization.CQRS.InitialBrokerRegistrationRequests.Dto;
using ITC.Authorization.Extensions;
using ITC.Authorization.Storage;
using ITC.CQRS.Base.Get;

namespace ITC.Authorization.CQRS.InitialBrokerRegistrationRequests;

public class Get : GetCollectionQueryBase
{
    public Guid? UserId { get; set; }

    public class Handler : UserGetCollectionQueryHandlerBase<ServiceDbContext, Get, InitialBrokerRegistrationRequest, InitialBrokerRegistrationRequestDto>
    {
        public Handler(IMapper mapper, ServiceDbContext db) : base(mapper, db)
        {
        }

        protected override ValueTask<IQueryable<InitialBrokerRegistrationRequest>> PreRequestAction(UserApiGetCollectionRequestWrapper<Get, InitialBrokerRegistrationRequestDto> request, IQueryable<InitialBrokerRegistrationRequest> query,
            CancellationToken cancellationToken)
        {
            if (!request.UserContext.IsAdmin())
                query = query.Where(c => c.UserId == request.UserContext.Id);

            return ValueTask.FromResult(query);
        }
    }
}