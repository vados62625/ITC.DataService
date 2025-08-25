using AutoMapper;
using FluentValidation;
using ITC.Domain.CQRS.Base;
using ITC.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.CQRS.Engine;

public class AddCommand : IRequest<EngineDto>
{
    public required string Mac { get; set; }
}

public class AddCommandHandler : CreateEntityHandlerBase<AddCommand, Models.Engine, EngineDto>
{
    public AddCommandHandler(IMapper mapper, DbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(mapper, dbContext, httpContextAccessor)
    {
    }

    protected override Task PreRequestAction(AddCommand request, IQueryable<Models.Engine> query, CancellationToken cancellationToken)
    {
        return Task.FromResult(query);
    }
}
public class AddCommandValidator : AbstractValidator<AddCommand>
{
    public AddCommandValidator(DbContext db)
    {
    }
}