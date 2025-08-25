using AutoMapper;
using FluentValidation;
using ITC.Domain.Dto;
using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Update;

/// <summary>
/// Базовый обработчик команды обновления сущности в БД через транзакцию
/// </summary>
/// <typeparam name="TCommand">Тип команды обновления сущности</typeparam>
/// <typeparam name="TDbContext">Тип контекста БД</typeparam>
/// <typeparam name="TWrapper">Тип обертки запроса</typeparam>
/// <typeparam name="TEntity">Тип сущности БД</typeparam>
/// <typeparam name="TResponse">Тип ответа (DTO)</typeparam>
public class UpdateEntityCommandTransactionHandlerBase<TDbContext, TWrapper, TCommand, TEntity, TResponse> : UpdateEntityCommandHandlerBase<TDbContext, TWrapper, TCommand, TEntity, TResponse>
    where TDbContext : DbContext
    where TWrapper : UserApiRequestWrapper<TCommand, TResponse>
    where TCommand : class, IUpdateEntityCommandBase
    where TEntity : class, IEntity
    where TResponse : class, IEntityDto
{
    private readonly IValidator<TWrapper> _validator;

    public UpdateEntityCommandTransactionHandlerBase(IMapper mapper, TDbContext db, IValidator<TWrapper> validator) 
        : base(mapper, db, validator)
    {
        _validator = validator;
    }
    public override async Task<TResponse> Handle(TWrapper requestWrapper, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(requestWrapper, cancellationToken);
        
        await using var transaction = await Db.Database.BeginTransactionAsync(cancellationToken);
        
        var result = await HandleRequest(requestWrapper, cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return result;
    }
}