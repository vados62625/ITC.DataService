using System.ComponentModel.DataAnnotations;
using Keycloak.Net.Core.Models.Users;
using MediatR;

namespace ITC.Authorization.CQRS;

public class UserSetEnabledCommand : IRequest<User>
{ 
    public Guid UserId { get; set; }
    public bool Enabled { get; set; }

    public class Handler : IRequestHandler<UserSetEnabledCommand, User>
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<User> Handle(UserSetEnabledCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetUserQuery(request.UserId), cancellationToken);
            if (user == null)
                throw new ValidationException("Пользователь не найден");
            
            user.Enabled = request.Enabled;
            var command = new KeyCloakUpdateUserCommand
            {
                User = user
            };

            return await _mediator.Send(command, cancellationToken);
        }
    }
}