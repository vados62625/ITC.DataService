using FluentValidation;
using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Extensions;

public static class ValidatorExtensions
{
    private const string EntityMustExistValidationMessage = "Entity {0} with ID {1} not found in the database";
    
    /// <summary>
    /// Проверяет наличие сущности в БД
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <param name="dbSet"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, Guid> EntityMustExistAsync<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        DbSet<TEntity> dbSet)
        where TEntity : class, IEntity
    {
        return ruleBuilder.MustAsync(async (id, ct) =>
                await dbSet.AnyAsync(e => e.Id == id, ct))
            .WithMessage((query, id) => string.Format(EntityMustExistValidationMessage, typeof(TEntity).Name, id));
    }

    /// <summary>
    /// Проверяет наличие сущности в БД
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <param name="dbSet"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, Guid?> EntityMustExistAsync<T, TEntity>(
        this IRuleBuilder<T, Guid?> ruleBuilder,
        DbSet<TEntity> dbSet)   
        where TEntity : class, IEntity
    {
        return ruleBuilder.MustAsync(async (id, ct) =>
            {
                if (id == null)
                    return true;
                return await dbSet.AnyAsync(e => e.Id == id, ct);
            })
            .WithMessage((query, id) => string.Format(EntityMustExistValidationMessage, typeof(TEntity).Name, id));
    }
}