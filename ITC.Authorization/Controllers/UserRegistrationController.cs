using Asp.Versioning;
using ITC.Authorization.CQRS;
using ITC.CQRS.Attributes;
using ITC.CQRS.Base.Controllers;
using ITC.CQRS.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITC.Authorization.Controllers;

[ApiRoute("Users")]
[ApiVersion("1")]
public class UserRegistrationController : RestControllerBase
{
    private readonly IMediator _mediator;

    public UserRegistrationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost, NoContent]
    public async Task<ActionResult> Registration([FromBody] SignupCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("InvitedUser"), Ok]
    public async Task<ActionResult> Registration([FromBody] SignupInvitedUserCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("VerifyEmail"), NoContent]
    public async Task<ActionResult> VerifyEmail([FromBody] CheckEmailVerificationCode command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("ResendEmailVerificationCode"), NoContent]
    public async Task<ActionResult> ResendEmailVerificationCode(CancellationToken cancellationToken)
    {
        var userEmail = User.GetUserEmail();
        var command = new ResendEmailVerificationCodeCommand(userEmail);

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}