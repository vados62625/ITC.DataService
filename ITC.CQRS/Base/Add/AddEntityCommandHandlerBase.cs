using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using ITC.Domain.Dto;
using ITC.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Add;

/// <summary>
/// Базовый обработчик команды добавления сущности в БД
/// </summary>
/// <typeparam name="TCommand">Тип команды создания сущности</typeparam>
/// <typeparam name="TDbContext">Тип контекста БД</typeparam>
/// <typeparam name="TWrapper">Тип обертки запроса</typeparam>
/// <typeparam name="TEntity">Тип сущности БД</typeparam>
/// <typeparam name="TResponse">Тип ответа (DTO)</typeparam>
public abstract class AddEntityCommandHandlerBase<TDbContext, TWrapper, TCommand, TEntity, TResponse> 
    : IRequestHandler<TWrapper, TResponse>

    where TDbContext : DbContext
    where TWrapper : UserApiRequestWrapper<TCommand, TResponse>
    where TCommand : class, IAddEntityCommandBase
    where TEntity : class, IEntity
    where TResponse : class, IEntityDto
{
    private readonly IValidator<TWrapper> _validator;
    protected IMapper Mapper { get; }
    protected TDbContext Db { get; }

    protected AddEntityCommandHandlerBase(IMapper mapper, TDbContext db, IValidator<TWrapper> validator)
    {
        _validator = validator;
        Mapper = mapper;
        Db = db;
    }
    
    public virtual async Task<TResponse> Handle(TWrapper request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        return await HandleRequest(request, cancellationToken);
    }
    
    protected async Task<TResponse> HandleRequest(TWrapper requestWrapper, CancellationToken cancellationToken)
    {
        var entity = Mapper.Map<TEntity>(requestWrapper.Request);

        entity = await PreRequestAction(requestWrapper, entity, cancellationToken);

        await Db.Set<TEntity>().AddAsync(entity, cancellationToken);
        await Db.SaveChangesAsync(cancellationToken);
        
        entity = await PostRequestAction(requestWrapper, entity, cancellationToken);
        await Db.SaveChangesAsync(cancellationToken);
        
        var result = await Db.Set<TEntity>()
            .AsTracking()
            .ProjectTo<TResponse>(Mapper.ConfigurationProvider)
            .FirstAsync(c => c.Id == entity.Id, cancellationToken);
        
        return result;
    }

    /// <summary>
    /// Действия, выполняемые перед запросом в БД
    /// </summary>
    /// <param name="requestWrapper"></param>
    /// <param name="entity">Сущность после маппинга из команды</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual ValueTask<TEntity> PreRequestAction(TWrapper requestWrapper, TEntity entity, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(entity);
    }    
    
    /// <summary>
    /// Действия, выполняемые после запрос в БД
    /// </summary>
    /// <param name="requestWrapper"></param>
    /// <param name="entity">Сущность после маппинга из команды</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual ValueTask<TEntity> PostRequestAction(TWrapper requestWrapper, TEntity entity, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(entity);
    }


}