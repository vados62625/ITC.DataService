using AutoMapper;
using ITC.Authorization.Storage;
using ITC.CQRS.Base.Suggest;

namespace ITC.Authorization.CQRS.InitialBrokerRegistrationRequests;

public class SuggestQuery : ISuggestRequestBase
{
    public Guid? Id { get; set; }
    public int MaxItems { get; set; }
    public string? Suggest { get; set; }

    public class Handler : SuggestRequestHandlerBase<ServiceDbContext, SuggestQuery, InitialBrokerRegistrationRequest>
    {
        public Handler(IMapper mapper, ServiceDbContext db) : base(mapper, db)
        {
        }

        protected override ValueTask<IQueryable<InitialBrokerRegistrationRequest>> HandleSuggest(IQueryable<InitialBrokerRegistrationRequest> query, UserApiSuggestRequestWrapper<SuggestQuery> requestWrapper,
            CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(query);
        }
    }
}