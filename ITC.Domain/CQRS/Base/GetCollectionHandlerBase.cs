using AutoMapper;
using AutoMapper.QueryableExtensions;
using ITC.Domain.Dto;
using ITC.Domain.Extensions;
using ITC.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.CQRS.Base;

public abstract class
    GetCollectionHandlerBase<TRequest, TEntity, TDto> : IRequestHandler<TRequest, PageableCollection<TDto>>
    where TRequest : class, IRequest<PageableCollection<TDto>>, IPagingQuery
    where TEntity : EntityBase
    where TDto : class
{
    protected readonly IMapper Mapper;
    protected readonly DbContext DbContext;
    protected readonly HttpContext HttpContext;

    protected GetCollectionHandlerBase(IMapper mapper, DbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        Mapper = mapper;
        DbContext = dbContext;
        HttpContext = httpContextAccessor.HttpContext!;
    }

    public virtual async Task<PageableCollection<TDto>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var query = DbContext.Set<TEntity>()
            .AsNoTracking();

        query = await PreRequestAction(request, query, cancellationToken);

        query = query.FilterBy(request);

        // query = OrderBy(request, query);

        var dtoQuery = await Project(request, query);

        var collection = await dtoQuery
            .ToPageableCollectionAsync(request, cancellationToken);

        collection = await PostRequestAction(request, collection, cancellationToken);

        return collection;
    }
    
    public abstract Task<IQueryable<TEntity>> PreRequestAction(TRequest request, IQueryable<TEntity> query,
        CancellationToken cancellationToken);

    public virtual Task<PageableCollection<TDto>> PostRequestAction(TRequest request,
        PageableCollection<TDto> collection, CancellationToken cancellationToken)
    {
        return Task.FromResult(collection);
    }

    protected virtual Task <IQueryable<TDto>> Project(TRequest request, IQueryable<TEntity> query)
    {
        return Task.FromResult(query.ProjectTo<TDto>(Mapper.ConfigurationProvider));
    }
}