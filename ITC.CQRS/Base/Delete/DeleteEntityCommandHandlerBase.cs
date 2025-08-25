using AutoMapper;
using FluentValidation;
using ITC.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Delete;

/// <summary>
/// Базовый обработчик команды обновления сущности в БД
/// </summary>
/// <typeparam name="TCommand">Тип команды обновления сущности</typeparam>
/// <typeparam name="TDbContext">Тип контекста БД</typeparam>
/// <typeparam name="TWrapper">Тип обертки запроса</typeparam>
/// <typeparam name="TEntity">Тип сущности БД</typeparam>
public abstract class DeleteEntityCommandHandlerBase<TDbContext, TWrapper, TCommand, TEntity> 
    : IRequestHandler<TWrapper>
    where TDbContext : DbContext
    where TWrapper : UserApiRequestWrapper<TCommand>
    where TCommand : DeleteEntityCommandBase
    where TEntity : class, IEntity
{
    protected IMapper Mapper { get; }
    protected TDbContext Db { get; }
    private readonly IValidator<TWrapper> _validator;

    protected DeleteEntityCommandHandlerBase(IMapper mapper, TDbContext db, IValidator<TWrapper> validator)
    {
        Mapper = mapper;
        Db = db;
        _validator = validator;
    }
    public virtual async Task Handle(TWrapper requestWrapper, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(requestWrapper, cancellationToken);
        await HandleRequest(requestWrapper, cancellationToken);
    }
    
    protected async Task HandleRequest(TWrapper requestWrapper, CancellationToken cancellationToken)
    {
        var entity = await Db.Set<TEntity>()
            .FirstAsync(c => c.Id == requestWrapper.Request.Id, cancellationToken);

        await PreRequestAction(requestWrapper, entity, cancellationToken);

        Db.Set<TEntity>().Remove(entity);

        await Db.SaveChangesAsync(cancellationToken);

        await PostRequestAction(requestWrapper, entity, cancellationToken);
        
    }
    /// <summary>
    /// Действия, выполняемые перед запросом в БД
    /// </summary>
    /// <param name="requestWrapper"></param>
    /// <param name="entity">Сущность извлеченная из БД</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual ValueTask PreRequestAction(TWrapper requestWrapper, TEntity entity, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Действия, выполняемые после запроса в БД
    /// </summary>
    /// <param name="requestWrapper"></param>
    /// <param name="entity">Сущность извлеченная из БД</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual ValueTask PostRequestAction(TWrapper requestWrapper, TEntity entity, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}