using Asp.Versioning;
using ITC.Authorization.CQRS;
using ITC.Authorization.CQRS.ExternalAuthTokens;
using ITC.Authorization.Storage;
using ITC.CQRS.Attributes;
using ITC.CQRS.Base;
using ITC.CQRS.Base.Controllers;
using ITC.CQRS.Extensions;
using Keycloak.Net.Core.Models.AccessToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITC.Authorization.Controllers;

[ApiRoute]
[ApiVersion("1")]
public class AuthorizationController : RestControllerBase
{
    private readonly IMediator _mediator;

    public AuthorizationController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [AllowAnonymous]
    [HttpPost("Signin"), Unauth, Ok]
    public async Task<ActionResult<AccessToken>> Signin([FromBody] SigninCommand command, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(command, cancellationToken);
        if (token == null)
            return Unauthorized();

        return token;
    }

    [Authorize(Policy = "users")]
    [HttpPost("Signout"), Unauth, Ok]
    public async Task<ActionResult> Signout(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var command = new SignoutCommand(userId);
        await _mediator.Send(command, cancellationToken);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("Refresh"), Unauth, Ok]
    public async Task<ActionResult<AccessToken>> Refresh([FromBody] RefreshCommand command, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(command, cancellationToken);
        if (token == null)
            return Unauthorized();
        return token;
    }

    [HttpPost("GenerateExternalAuthToken"), Unauth, Ok]
    public async Task<ActionResult<ExternalAuthToken>> GenerateExternalAuthToken([FromBody] GenerateToken command, CancellationToken cancellationToken)
    {
        UserApiRequestWrapper<GenerateToken, ExternalAuthToken> wrapper =
            new UserApiRequestWrapper<GenerateToken, ExternalAuthToken>(command, HttpContext);
        var token = await _mediator.Send(wrapper, cancellationToken);
        return token;
    }
}