using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using ITC.Domain.Extensions;
using ITC.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.CQRS.Base;

public abstract class UpdateEntityCommandBase<TDto> : IRequest<TDto>
{
    [Required]
    public Guid Id { get; set; }
}

public abstract class UpdateEntityHandlerBase<TCommand, TEntity, TDto> : IRequestHandler<TCommand, TDto>
where TCommand : UpdateEntityCommandBase<TDto>
where TEntity : EntityBase
{
    protected readonly DbContext Db;
    protected readonly IMapper Mapper;
    private readonly HttpContext _httpContext;
    private readonly IValidator<TCommand> _validator;

    protected UpdateEntityHandlerBase(DbContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IValidator<TCommand> validator)
    {
        Db = db;
        Mapper = mapper;
        _httpContext = httpContextAccessor.HttpContext!;
        _validator = validator;
    }

    public virtual async Task<TDto> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var entity = await Db.Set<TEntity>()
            .FirstAsync(c => c.Id == command.Id, cancellationToken);

        Mapper.Map(command, entity);

        entity =  await PreRequestAction(command, entity, cancellationToken);
        Db.Set<TEntity>().Update(entity);
        await Db.SaveChangesAsync(cancellationToken);

        var dto = await Db.Set<TEntity>()
            .AsNoTracking()
            .Where(c => c.Id == command.Id)
            .ProjectTo<TDto>(Mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
        
        dto = await PostRequestAction(command, dto, cancellationToken);

        return dto;
    }

    protected abstract Task<TEntity> PreRequestAction(TCommand command, TEntity entity, CancellationToken cancellationToken);

    protected virtual Task<TDto> PostRequestAction(TCommand command, TDto dto, CancellationToken cancellationToken)
    {
        return Task.FromResult(dto);   
    }
}

public abstract class UpdateEntityValidatorBase<TCommand, TEntity, TDto> : AbstractValidator<TCommand>
    where TCommand : UpdateEntityCommandBase<TDto>
    where TEntity : EntityBase
{
    protected UpdateEntityValidatorBase(DbContext db)
    {
        RuleFor(c => c.Id).EntityMustExistAsync(db.Set<TEntity>());
    }
}