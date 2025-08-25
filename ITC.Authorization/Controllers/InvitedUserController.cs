using Asp.Versioning;
using ITC.Authorization.CQRS;
using ITC.Authorization.CQRS.InviteUsers;
using ITC.Authorization.CQRS.InviteUsers.Dto;
using ITC.CQRS.Attributes;
using ITC.CQRS.Base.Controllers;
using ITC.CQRS.Base.Get;
using ITC.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITC.Authorization.Controllers;

[ApiRoute]
[ApiVersion("1")]
public class InvitedUserController : RestControllerBase
{
    private readonly IMediator _mediator;
    public InvitedUserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet, Ok]
    public async Task<ActionResult<IPageableCollection<InvitedUserDto>>> GetCollection([FromQuery] Get query, CancellationToken cancellationToken)
    {
        UserApiGetCollectionRequestWrapper<Get, InvitedUserDto> request2 = new UserApiGetCollectionRequestWrapper<Get, InvitedUserDto>(query, base.HttpContext);
        return Ok(await _mediator.Send(request2, cancellationToken));
    }

    [HttpPost, Ok]
    public async Task<ActionResult<CreatedInvitedUser>> Add([FromBody] AddInvitedUserCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}