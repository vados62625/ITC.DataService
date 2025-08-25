using ITC.CQRS.Base.Add;
using ITC.CQRS.Base.Delete;
using ITC.CQRS.Base.Suggest;
using ITC.CQRS.Base.Update;
using ITC.Domain.Dto;
using ITC.Storage.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITC.CQRS.Base.Controllers;

/// <summary>
/// Базовый контроллер CRUD операций для сущности
/// </summary>
/// <typeparam name="TGetRequest"></typeparam>
/// <typeparam name="TAddCommand"></typeparam>
/// <typeparam name="TUpdateCommand"></typeparam>
/// <typeparam name="TDeleteCommand"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TSuggestRequest"></typeparam>
public abstract class CrudControllerBase <TGetRequest, TSuggestRequest, TAddCommand, TUpdateCommand, TDeleteCommand, TResponse> 
    : CreateControllerBase<TGetRequest, TSuggestRequest, TAddCommand, TResponse>
    where TResponse : class, IEntityDto
    where TGetRequest : class, IEntityQuery
    where TSuggestRequest : class, ISuggestRequestBase
    where TAddCommand : class, IAddEntityCommandBase
    where TUpdateCommand : class, IUpdateEntityCommandBase
    where TDeleteCommand : DeleteEntityCommandBase
{
    private readonly IMediator _mediator;

    protected CrudControllerBase(IMediator mediator) : base(mediator)
    {
        _mediator = mediator;
    }
    
    protected async Task<ActionResult<TResponse>> Update([FromBody] TUpdateCommand command,
        CancellationToken cancellationToken)
    {
        var wrapper = new UserApiRequestWrapper<TUpdateCommand, TResponse>(command, HttpContext);
        var result = await _mediator.Send(wrapper, cancellationToken);
        return result;
    }

    protected async Task<ActionResult> Delete([FromBody] TDeleteCommand command,
        CancellationToken cancellationToken)    
    {
        var wrapper = new UserApiRequestWrapper<TDeleteCommand>(command, HttpContext);
        await _mediator.Send(wrapper, cancellationToken);
        return NoContent();
    }

    public abstract Task<ActionResult<TResponse>> UpdateEntity([FromBody] TUpdateCommand command,
        CancellationToken cancellationToken);

    public abstract Task<ActionResult> DeleteEntity([FromBody] TDeleteCommand command,
        CancellationToken cancellationToken);
}