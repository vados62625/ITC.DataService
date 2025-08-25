using FluentValidation;
using ITC.CQRS.Extensions;
using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Delete;

public abstract class DeleteEntityCommandValidatorBase<TDbContext, TWrapper, TCommand, TEntity> : AbstractValidator<TWrapper>
    where TDbContext : DbContext
    where TWrapper : UserApiRequestWrapper<TCommand>
    where TEntity : class, IEntity
    where TCommand : DeleteEntityCommandBase
{

    protected DeleteEntityCommandValidatorBase(TDbContext db)
    {
        RuleFor(c => c.Request.Id)
            .EntityMustExistAsync(db.Set<TEntity>());
    }
}