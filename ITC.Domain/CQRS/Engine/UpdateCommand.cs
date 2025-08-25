using AutoMapper;
using FluentValidation;
using ITC.Domain.CQRS.Base;
using ITC.Domain.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.CQRS.Engine;

public class UpdateCommand : UpdateEntityCommandBase<EngineDto>
{
}

public class UpdateCommandHandler : UpdateEntityHandlerBase<UpdateCommand, Models.Engine, EngineDto>
{
    public UpdateCommandHandler(DbContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IValidator<UpdateCommand> validator) : base(db, mapper, httpContextAccessor, validator)
    {
    }

    protected override Task<Models.Engine> PreRequestAction(UpdateCommand command, Models.Engine entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(entity);
    }
}
public class UpdateCommandValidator : UpdateEntityValidatorBase<UpdateCommand, Models.Engine, EngineDto>
{
    public UpdateCommandValidator(DbContext db) : base(db)
    {
    }
}