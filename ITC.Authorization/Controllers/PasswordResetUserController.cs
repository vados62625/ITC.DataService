using Asp.Versioning;
using ITC.Authorization.CQRS;
using ITC.CQRS.Attributes;
using ITC.CQRS.Base;
using ITC.CQRS.Base.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITC.Authorization.Controllers;

[ApiRoute]
[ApiVersion("1")]
public class PasswordResetUserController : RestControllerBase
{
    private readonly IMediator _mediator;
    public PasswordResetUserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("CheckPasswordResetToken"), NoContent]
    public async Task<ActionResult> CheckPasswordResetToken([FromBody] CheckPasswordResetTokenCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("AddTokenPasswordResetUser"), Ok]
    public async Task<ActionResult> AddPasswordResetUserCommand([FromBody] AddPasswordResetUserCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpPost("PasswordResetUser"), Ok]
    public async Task<ActionResult> PasswordResetUserCommand([FromBody] PasswordResetUserCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpPost("PasswordResetAuthorizedUser"), Ok]
    public async Task<ActionResult> PasswordResetAuthorizedUserCommand([FromBody] PasswordResetAuthorizedUserCommand command, CancellationToken cancellationToken)
    {
        var requestWrapper = new UserApiRequestWrapper<PasswordResetAuthorizedUserCommand>(command, HttpContext);
        await _mediator.Send(requestWrapper, cancellationToken);
        return NoContent();
    }
}