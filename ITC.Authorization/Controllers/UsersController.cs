using Asp.Versioning;
using ITC.Authorization.CQRS;
using ITC.CQRS.Attributes;
using ITC.CQRS.Base;
using ITC.CQRS.Base.Controllers;
using ITC.CQRS.Extensions;
using ITC.Domain.Dto;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITC.Authorization.Controllers;

[ApiRoute, Unauth]
[ApiVersion("1")]
public class UsersController : RestControllerBase
{
    private readonly KeycloakClient _keycloak;
    private readonly IMediator _mediator;

    public UsersController(KeycloakClient keycloak, IMediator mediator)
    {
        _keycloak = keycloak;
        _mediator = mediator;
    }

    [HttpGet, Ok]
    [Authorize(Policy = "UsersRead")]
    public async Task<ActionResult<PageableCollection<User>>> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(query, cancellationToken);
        return users;
    }

    [HttpGet("Current"), Ok, NotFound]
    [Authorize(Policy = "users")]
    public async Task<ActionResult<User>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var query = new GetUserQuery(userId);
        var user = await _mediator.Send(query, cancellationToken);

        if (user == null)
            return NotFound();

        return user;
    }

    [HttpGet("AuthorizedContext"), Ok]
    [Authorize(Policy = "users")]
    public ActionResult<UserContext> GetAuthorizedContext()
    {
        return User.GetUserContext();
    }

    [HttpPatch("CompanyIdAttribute"), Ok]
    public async Task <ActionResult<User>> SetCompanyId(UserSetCompanyIdCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPatch("BrokerIdAttribute"), Ok]
    public async Task<ActionResult<User>> SetBrokerId(UserSetBrokerIdCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPatch("EmployeeIdAttribute"), Ok]
    // [Authorize(Policy = AuthPolices.AdminsAndHigher)]
    public async Task<ActionResult<User>> SetEmployeeId(UserSetEmployeeIdCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
    
    [HttpPatch("SetEnabled"), Ok]
    public async Task<ActionResult<User>> SetEnabled(UserSetEnabledCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
    
    [HttpDelete, NoContent]
    public async Task<ActionResult> Delete(UserDeleteCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpPatch("Email"), Ok]
    public async Task<ActionResult> ChangeEmail(ChangeUserEmail command, CancellationToken cancellationToken)
    {
        var requestWrapper = new UserApiRequestWrapper<ChangeUserEmail>(command, HttpContext);
        await _mediator.Send(requestWrapper, cancellationToken);
        return NoContent();
    }
    
    [HttpPatch("Roles"), Ok]
    public async Task<ActionResult> SetRoles(UserSetRolesCommand command, CancellationToken cancellationToken)
    {
        var requestWrapper = new UserApiRequestWrapper<UserSetRolesCommand>(command, HttpContext);
        await _mediator.Send(requestWrapper, cancellationToken);
        return NoContent();
    }
}