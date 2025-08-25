using AutoMapper;
using AutoMapper.QueryableExtensions;
using ITC.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.CQRS.Base;

public abstract class CreateEntityHandlerBase<TRequest, TEntity, TDto> : IRequestHandler<TRequest, TDto>
    where TRequest : class, IRequest<TDto>
    where TEntity : EntityBase
{
    private readonly IMapper _mapper;
    private readonly DbContext _dbContext;
    private readonly HttpContext _httpContext;

    protected CreateEntityHandlerBase(IMapper mapper, DbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public virtual async Task<TDto> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<TEntity>();
        var query = dbSet.AsQueryable();
            await PreRequestAction(request, query, cancellationToken);

        var entity = _mapper.Map<TEntity>(request);
        await dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        var dto = await query
            .Where(c => c.Id == entity.Id)
            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
        return dto;
    }

    protected abstract Task PreRequestAction(TRequest request, IQueryable<TEntity> query,
        CancellationToken cancellationToken);
}