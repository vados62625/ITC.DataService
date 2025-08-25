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
    GetArrayHandlerBase<TRequest, TEntity, TDto> : IRequestHandler<TRequest, TDto[]>
    where TRequest : class, IRequest<TDto[]>
    where TDto : EntityDtoBase
    where TEntity : EntityBase
{
    private readonly IMapper _mapper;
    private readonly DbContext _dbContext;
    private readonly HttpContext _httpContext;

    protected GetArrayHandlerBase(IMapper mapper, DbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<TDto[]> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<TEntity>()
            .AsNoTracking();

        query = await PreRequestAction(request, query, cancellationToken);

        query = query.FilterBy(request);

        // query = OrderBy(request, query);

        var dtoQuery = await Project(request, query);

        var collection = await dtoQuery
            .FilterBy(request)
            .ToArrayAsync(cancellationToken);

        return await PostRequestAction(request, collection, cancellationToken);
    }
    
    public abstract Task<IQueryable<TEntity>> PreRequestAction(TRequest request, IQueryable<TEntity> query,
        CancellationToken cancellationToken);

    public virtual Task<TDto[]> PostRequestAction(TRequest request,
        TDto[] collection, CancellationToken cancellationToken)
    {
        return Task.FromResult(collection);
    }

    protected virtual Task <IQueryable<TDto>> Project(TRequest request, IQueryable<TEntity> query)
    {
        return Task.FromResult(query.ProjectTo<TDto>(_mapper.ConfigurationProvider));
    }
}