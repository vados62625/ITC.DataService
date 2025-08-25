using AutoMapper;
using FluentValidation;
using ITC.Domain.Dto;
using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Add;

/// <summary>
/// Базовый обработчик команды добавления сущности в БД через транзакцию
/// </summary>
/// <typeparam name="TCommand">Тип команды создания сущности</typeparam>
/// <typeparam name="TDbContext">Тип контекста БД</typeparam>
/// <typeparam name="TWrapper">Тип обертки запроса</typeparam>
/// <typeparam name="TEntity">Тип сущности БД</typeparam>
/// <typeparam name="TResponse">Тип ответа (DTO)</typeparam>
public abstract class AddEntityCommandTransactionHandlerBase<TDbContext, TWrapper, TCommand, TEntity, TResponse>  
    : AddEntityCommandHandlerBase<TDbContext, TWrapper, TCommand, TEntity, TResponse> 
    where TDbContext : DbContext
    where TWrapper : UserApiRequestWrapper<TCommand, TResponse>
    where TCommand : class, IAddEntityCommandBase
    where TEntity : class, IEntity
    where TResponse : class, IEntityDto
{
    private readonly IValidator<TWrapper> _validator;

    protected AddEntityCommandTransactionHandlerBase(IMapper mapper, TDbContext db, IValidator<TWrapper> validator) 
        : base(mapper, db, validator)
    {
        _validator = validator;
    }   

    public override async Task<TResponse> Handle(TWrapper request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        
        await using var transaction = await Db.Database.BeginTransactionAsync(cancellationToken);
        
        var result = await HandleRequest(request, cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return result;
    }
}