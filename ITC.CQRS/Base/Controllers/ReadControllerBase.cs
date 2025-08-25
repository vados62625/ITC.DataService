using ITC.CQRS.Base.Get;
using ITC.CQRS.Base.Suggest;
using ITC.Domain.Dto;
using ITC.Storage.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITC.CQRS.Base.Controllers;

/// <summary>
/// Базовый контроллер для операции Read 
/// </summary>
/// <typeparam name="TGetRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TSuggestRequest"></typeparam>
public abstract class ReadControllerBase<TGetRequest, TSuggestRequest, TResponse> : RestControllerBase
    where TResponse : class, IEntityDto
    where TGetRequest : class, IEntityQuery
    where TSuggestRequest : class, ISuggestRequestBase
{
    protected readonly IMediator Mediator;

    protected ReadControllerBase(IMediator mediator)
    {
        Mediator = mediator;
    }

    protected async Task<ActionResult<IPageableCollection<TResponse>>> Get(TGetRequest request,
        CancellationToken cancellationToken)
    {
        var wrapper = new UserApiGetCollectionRequestWrapper<TGetRequest, TResponse>(request, HttpContext);
        var collection = await Mediator.Send(wrapper, cancellationToken);
        return Ok(collection);
    }

    protected async Task<ActionResult<SuggestResultDto[]>> ApplySuggest(TSuggestRequest request,
        CancellationToken cancellationToken)
    {
        var wrapper = new UserApiSuggestRequestWrapper<TSuggestRequest>(request, HttpContext);
        var collection = await Mediator.Send(wrapper, cancellationToken);
        return Ok(collection);
    }


    public abstract Task<ActionResult<IPageableCollection<TResponse>>> GetCollection(TGetRequest request,
        CancellationToken cancellationToken);

    public abstract Task<ActionResult<SuggestResultDto[]>> Suggest(TSuggestRequest request,
        CancellationToken cancellationToken);
}