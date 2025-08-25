using Keycloak.Net.Core.Models.Users;
using MediatR;
using NLog;
using ILogger = NLog.ILogger;


namespace ITC.Authorization.CQRS;

public class UserSetCompanyIdCommand : IRequest<User>
{
    private const string AttributeKey = "companyid";

    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }

    public class Handler : IRequestHandler<UserSetCompanyIdCommand, User>
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<User> Handle(UserSetCompanyIdCommand request, CancellationToken cancellationToken)
        {
            var command = new KeyCloakSetAttributeCommand
            {
                UserId = request.UserId,
                Key = AttributeKey,
                Value = request.CompanyId.ToString()
            };

            return await _mediator.Send(command, cancellationToken);
        }
    }
}