using FluentValidation;
using ITC.Domain.CQRS.Base;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.CQRS.Engine;


public class DeleteCommand : DeleteEntityCommandBase
{
}

public class DeleteCommandHandler : DeleteEntityHandlerBase<DeleteCommand, Models.Engine>
{
    public DeleteCommandHandler(DbContext db, IValidator<DeleteCommand> validator) : base(db, validator)
    {
    }
}
public class
    DeleteCommandValidator : DeleteEntityCommandValidatorBase<Models.Engine, DeleteCommand>
{
    public DeleteCommandValidator(DbContext db) : base(db)
    {
    }
}