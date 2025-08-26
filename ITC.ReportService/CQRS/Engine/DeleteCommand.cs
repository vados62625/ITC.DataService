using FluentValidation;
using ITC.Domain.CQRS.Base;
using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.CQRS.Engine;


public class DeleteCommand : DeleteEntityCommandBase
{
}

public class DeleteCommandHandler : DeleteEntityHandlerBase<DeleteCommand, Domain.Models.Engine>
{
    public DeleteCommandHandler(DbContext db, IValidator<DeleteCommand> validator) : base(db, validator)
    {
    }
}
public class
    DeleteCommandValidator : DeleteEntityCommandValidatorBase<Domain.Models.Engine, DeleteCommand>
{
    public DeleteCommandValidator(DbContext db) : base(db)
    {
    }
}