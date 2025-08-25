using Asp.Versioning;
using ITC.Authorization.CQRS.UserRolesCaches;
using ITC.Authorization.CQRS.UserRolesCaches.Dto;
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
public class UserRolesController : ReadControllerBase<Get, SuggestQuery, UserRolesCacheDto>
{
    public UserRolesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet, Ok]
    [Authorize(Policy = "users")]
    public override Task<ActionResult<IPageableCollection<UserRolesCacheDto>>> GetCollection([FromQuery] Get request,
        CancellationToken cancellationToken) => Get(request, cancellationToken);

    [NonAction]
    public override Task<ActionResult<SuggestResultDto[]>> Suggest([FromQuery] SuggestQuery request,
        CancellationToken cancellationToken) => ApplySuggest(request, cancellationToken);
}
