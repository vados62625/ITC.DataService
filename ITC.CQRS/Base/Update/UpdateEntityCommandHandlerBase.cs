using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using ITC.Domain.Dto;
using ITC.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Update;

/// <summary>
/// Базовый обработчик команды обновления сущности в БД
/// </summary>
/// <typeparam name="TCommand">Тип команды обновления сущности</typeparam>
/// <typeparam name="TDbContext">Тип контекста БД</typeparam>
/// <typeparam name="TWrapper">Тип обертки запроса</typeparam>
/// <typeparam name="TEntity">Тип сущности БД</typeparam>
/// <typeparam name="TResponse">Тип ответа (DTO)</typeparam>
public abstract class UpdateEntityCommandHandlerBase<TDbContext, TWrapper, TCommand, TEntity, TResponse> 
    : IRequestHandler<TWrapper, TResponse>
    where TDbContext : DbContext
    where TWrapper : UserApiRequestWrapper<TCommand, TResponse>
    where TCommand : class, IUpdateEntityCommandBase
    where TEntity : class, IEntity
    where TResponse : class, IEntityDto
{
    protected IMapper Mapper { get; }
    protected TDbContext Db { get; }
    private readonly IValidator<TWrapper> _validator;

    protected UpdateEntityCommandHandlerBase(IMapper mapper, TDbContext db, IValidator<TWrapper> validator)
    {
        Mapper = mapper;
        Db = db;
        _validator = validator;
    }
    public virtual async Task<TResponse> Handle(TWrapper requestWrapper, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(requestWrapper, cancellationToken);
        return await HandleRequest(requestWrapper, cancellationToken);
    }
    
    protected async Task<TResponse> HandleRequest(TWrapper requestWrapper, CancellationToken cancellationToken)
    {
        var entity = await Db.Set<TEntity>()
            .FirstAsync(c => c.Id == requestWrapper.Request.Id, cancellationToken);

        Mapper.Map(requestWrapper.Request, entity);

        entity = await PreRequestAction(requestWrapper, entity, cancellationToken);

        Db.Set<TEntity>().Update(entity);

        await Db.SaveChangesAsync(cancellationToken);

        entity = await PostRequestAction(requestWrapper, entity, cancellationToken);

        var resultQuery = Db.Set<TEntity>().AsTracking();

        return await Project(resultQuery)
            .FirstAsync(c => c.Id == requestWrapper.Request.Id, cancellationToken);
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

    protected virtual IQueryable<TResponse> Project(IQueryable<TEntity> query)
    {
        return query.ProjectTo<TResponse>(Mapper.ConfigurationProvider);
    }
}