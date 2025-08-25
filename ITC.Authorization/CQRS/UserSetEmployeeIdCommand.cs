using Keycloak.Net.Core.Models.Users;
using MediatR;
using NLog;
using ILogger = NLog.ILogger;


namespace ITC.Authorization.CQRS;

public class UserSetEmployeeIdCommand : IRequest<User>
{
    private const string AttributeKey = "employeeid";

    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public Guid UserId { get; set; }
    public Guid EmployeeId { get; set; }

    public class Handler : IRequestHandler<UserSetEmployeeIdCommand, User>
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<User> Handle(UserSetEmployeeIdCommand request, CancellationToken cancellationToken)
        {
            var command = new KeyCloakSetAttributeCommand
            {
                UserId = request.UserId,
                Key = AttributeKey,
                Value = request.EmployeeId.ToString()
            };

            return await _mediator.Send(command, cancellationToken);
        }
    }
}