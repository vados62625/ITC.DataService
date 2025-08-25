using AutoMapper;
using AutoMapper.QueryableExtensions;
using ITC.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Suggest;

public abstract class SuggestRequestHandlerBase<TDbContext, TRequest, TEntity> 
    : IRequestHandler<UserApiSuggestRequestWrapper<TRequest>, SuggestResultDto[]>
    where TEntity : class, IEntity
    where TDbContext : DbContext
    where TRequest : class, ISuggestRequestBase
{
    protected IMapper Mapper { get; }
    protected TDbContext Db { get; }

    protected SuggestRequestHandlerBase(IMapper mapper, TDbContext db)
    {
        Mapper = mapper;
        Db = db;
    }
    public async Task<SuggestResultDto[]> Handle(UserApiSuggestRequestWrapper<TRequest> requestWrapper, CancellationToken cancellationToken)
    {
        var query = Db.Set<TEntity>()
            .AsNoTracking();

        query = await PreRequestAction(query, requestWrapper, cancellationToken);

        query = query.OrderByDescending(c => c.CreatedAt);

        if (requestWrapper.Request.Id.HasValue)
        {
            query = query.Where(c => c.Id == requestWrapper.Request.Id);
        }
        else if (!string.IsNullOrEmpty(requestWrapper.Request.Suggest))
        {
            requestWrapper.Request.Suggest = requestWrapper.Request.Suggest.ToLower();
            query = await HandleSuggest(query, requestWrapper, cancellationToken);
        }

        var results = await Map(query, cancellationToken);

        return await results
            .Take(requestWrapper.Request.MaxItems)
            .ToArrayAsync(cancellationToken);
    }
    
    protected abstract ValueTask<IQueryable<TEntity>> HandleSuggest(IQueryable<TEntity> query, UserApiSuggestRequestWrapper<TRequest> requestWrapper, CancellationToken cancellationToken);

    /// <summary>
    /// По умолчанию работает через конфигурацию маппера.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual ValueTask<IQueryable<SuggestResultDto>> Map(IQueryable<TEntity> query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(query.ProjectTo<SuggestResultDto>(Mapper.ConfigurationProvider));
    }

    protected virtual ValueTask<IQueryable<TEntity>> PreRequestAction(IQueryable<TEntity> query,
        UserApiSuggestRequestWrapper<TRequest> requestWrapper, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(query);
    }
}