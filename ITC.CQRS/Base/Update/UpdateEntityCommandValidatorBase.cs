using FluentValidation;
using ITC.CQRS.Extensions;
using ITC.Domain.Dto;
using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Update;

public abstract class UpdateEntityCommandValidatorBase<TDbContext, TWrapper, TCommand, TEntity, TResponse> : AbstractValidator<TWrapper>
    where TDbContext : DbContext
    where TWrapper : UserApiRequestWrapper<TCommand, TResponse>
    where TEntity : class, IEntity
    where TResponse : IEntityDto
    where TCommand : class, IUpdateEntityCommandBase
{

    protected UpdateEntityCommandValidatorBase(TDbContext db)
    {
        RuleFor(c => c.Request.Id)
            .EntityMustExistAsync(db.Set<TEntity>());
    }
}