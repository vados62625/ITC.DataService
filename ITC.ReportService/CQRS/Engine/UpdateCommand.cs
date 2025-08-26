using AutoMapper;
using FluentValidation;
using ITC.Domain.CQRS.Base;
using ITC.Domain.Dto;
using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.CQRS.Engine;

public class UpdateCommand : UpdateEntityCommandBase<EngineDto>
{
    public string? Name { get; set; }
}

public class UpdateCommandHandler : UpdateEntityHandlerBase<UpdateCommand, Domain.Models.Engine, EngineDto>
{
    public UpdateCommandHandler(DbContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IValidator<UpdateCommand> validator) : base(db, mapper, httpContextAccessor, validator)
    {
    }

    protected override Task<Domain.Models.Engine> PreRequestAction(UpdateCommand command, Domain.Models.Engine entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(entity);
    }
}
public class UpdateCommandValidator : UpdateEntityValidatorBase<UpdateCommand, Domain.Models.Engine, EngineDto>
{
    public UpdateCommandValidator(DbContext db) : base(db)
    {
    }
}