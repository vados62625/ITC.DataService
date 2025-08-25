using Asp.Versioning;
using ITC.Authorization.CQRS.InitialBrokerRegistrationRequests;
using ITC.Authorization.CQRS.InitialBrokerRegistrationRequests.Dto;
using ITC.CQRS.Attributes;
using ITC.CQRS.Base.Controllers;
using ITC.CQRS.Base.Suggest;
using ITC.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITC.Authorization.Controllers;

[ApiRoute]
[ApiVersion("1")]
public class InitialBrokerRegistrationRequestController : ReadControllerBase<Get, SuggestQuery, InitialBrokerRegistrationRequestDto>
{
    public InitialBrokerRegistrationRequestController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet, Ok]
    public override Task<ActionResult<IPageableCollection<InitialBrokerRegistrationRequestDto>>> GetCollection([FromQuery] Get request,
        CancellationToken cancellationToken) => Get(request, cancellationToken);

    [NonAction]
    public override Task<ActionResult<SuggestResultDto[]>> Suggest([FromQuery] SuggestQuery request,
        CancellationToken cancellationToken) => ApplySuggest(request, cancellationToken);

}