using ITC.CQRS.Base.Add;
using ITC.CQRS.Base.Suggest;
using ITC.Domain.Dto;
using ITC.Storage.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITC.CQRS.Base.Controllers;

/// <summary>
/// Базовый контроллер для операций Read и Create 
/// </summary>
/// <typeparam name="TGetRequest"></typeparam>
/// <typeparam name="TAddCommand"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TSuggestRequest"></typeparam>
public abstract class CreateControllerBase<TGetRequest, TSuggestRequest, TAddCommand, TResponse> : ReadControllerBase<TGetRequest, TSuggestRequest, TResponse>
    where TResponse : class, IEntityDto
    where TGetRequest : class, IEntityQuery
    where TSuggestRequest : class, ISuggestRequestBase
    where TAddCommand : class, IAddEntityCommandBase
{
    private readonly IMediator _mediator;

    protected CreateControllerBase(IMediator mediator) : base(mediator)
    {
        _mediator = mediator;
    }

    protected async Task<ActionResult<TResponse>> Add(TAddCommand command,
        CancellationToken cancellationToken)
    {
        var wrapper = new UserApiRequestWrapper<TAddCommand, TResponse>(command, HttpContext);
        var result = await _mediator.Send(wrapper, cancellationToken);
        return CreatedAtAction(nameof(GetCollection), new { result.Id }, result);
    }

    public abstract Task<ActionResult<TResponse>> AddEntity(TAddCommand command,
        CancellationToken cancellationToken);
}