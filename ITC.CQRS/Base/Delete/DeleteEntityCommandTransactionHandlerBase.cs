using AutoMapper;
using FluentValidation;
using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Delete;

/// <summary>
/// Базовый обработчик команды обновления сущности в БД через транзакцию
/// </summary>
/// <typeparam name="TCommand">Тип команды обновления сущности</typeparam>
/// <typeparam name="TDbContext">Тип контекста БД</typeparam>
/// <typeparam name="TWrapper">Тип обертки запроса</typeparam>
/// <typeparam name="TEntity">Тип сущности БД</typeparam>
public class DeleteEntityCommandTransactionHandlerBase<TDbContext, TWrapper, TCommand, TEntity> : DeleteEntityCommandHandlerBase<TDbContext, TWrapper, TCommand, TEntity> 
    where TDbContext : DbContext
    where TWrapper : UserApiRequestWrapper<TCommand>
    where TCommand : DeleteEntityCommandBase
    where TEntity : class, IEntity
{
    private readonly IValidator<TWrapper> _validator;

    public DeleteEntityCommandTransactionHandlerBase(IMapper mapper, TDbContext db, IValidator<TWrapper> validator) : base(mapper, db, validator)
    {
        _validator = validator;
    }
    public override async Task Handle(TWrapper requestWrapper, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(requestWrapper, cancellationToken);

        await using var transaction = await Db.Database.BeginTransactionAsync(cancellationToken);
        
        await HandleRequest(requestWrapper, cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
    }
}