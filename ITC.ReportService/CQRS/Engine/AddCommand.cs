using AutoMapper;
using FluentValidation;
using ITC.Domain.CQRS.Base;
using ITC.Domain.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.CQRS.Engine;

public class AddCommand : IRequest<EngineDto>
{
    public string? Name { get; set; }
}

public class AddCommandHandler : CreateEntityHandlerBase<AddCommand, Domain.Models.Engine, EngineDto>
{
    public AddCommandHandler(IMapper mapper, DbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(mapper, dbContext, httpContextAccessor)
    {
    }

    protected override Task PreRequestAction(AddCommand request, IQueryable<Domain.Models.Engine> query, CancellationToken cancellationToken)
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