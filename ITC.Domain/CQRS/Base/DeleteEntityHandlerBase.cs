using FluentValidation;
using ITC.Domain.Extensions;
using ITC.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.CQRS.Base;

public abstract class DeleteEntityCommandBase : IRequest
{
    public Guid Id { get; set; }
}

public abstract class DeleteEntityHandlerBase<TCommand, TEntity> : IRequestHandler<TCommand>
 where TEntity : EntityBase
 where TCommand : DeleteEntityCommandBase
{
    protected readonly DbContext Db;
    protected readonly IValidator<TCommand> Validator;

    protected DeleteEntityHandlerBase(DbContext db, IValidator<TCommand> validator)
    {
        Db = db;
        Validator = validator;
    }
    public virtual async Task Handle(TCommand request, CancellationToken cancellationToken)
    {
        await Validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await Db.Set<TEntity>().FirstAsync(c => c.Id == request.Id, cancellationToken);
        Db.Set<TEntity>().Remove(entity);
        await Db.SaveChangesAsync(cancellationToken);
    }
}

public abstract class DeleteEntityCommandValidatorBase<TEntity, TCommand> : AbstractValidator<TCommand>
    where TCommand : DeleteEntityCommandBase
    where TEntity : EntityBase
{
    protected DeleteEntityCommandValidatorBase(DbContext db)
    {
        RuleFor(c => c.Id).EntityMustExistAsync(db.Set<TEntity>());
    }
}
