using AutoMapper;
using ITC.Domain.CQRS.Base;
using ITC.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.CQRS.Engine;

public class GetQuery : PagingQueryBase, IRequest<PageableCollection<EngineDto>>
{
    public string? Mac { get; set; }
}

public class GetQueryHandler : GetCollectionHandlerBase<GetQuery, Models.Engine, EngineDto>
{
    public GetQueryHandler(IMapper mapper, DbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(mapper, dbContext, httpContextAccessor)
    {
    }

    public override Task<IQueryable<Models.Engine>> PreRequestAction(GetQuery request, IQueryable<Models.Engine> query, CancellationToken cancellationToken)
    {
        return Task.FromResult(query);
    }
}