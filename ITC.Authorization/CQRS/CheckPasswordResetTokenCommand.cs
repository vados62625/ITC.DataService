using FluentValidation;
using ITC.Authorization.CQRS.PasswordResetUsers;
using ITC.Authorization.CQRS.PasswordResetUsers.Dto;
using ITC.CQRS.Base.Get;
using MediatR;

namespace ITC.Authorization.CQRS;

public class CheckPasswordResetTokenCommand : IRequest
{
    public string? PasswordResetToken { get; set; }
    
    public class Handler(
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<CheckPasswordResetTokenCommand>
    {

        public async Task Handle(CheckPasswordResetTokenCommand request, CancellationToken cancellationToken)
        {
            var get = new Get()
            {
                PasswordResetToken = request.PasswordResetToken
            };
            
            var requestWrapperGet = new UserApiGetCollectionRequestWrapper<Get, PasswordResetUserDto>(get, httpContextAccessor.HttpContext);
            var collection = await mediator.Send(requestWrapperGet, cancellationToken);
            
            if(collection.Items.Count == 0)
                throw new ValidationException("Ссылка для смены пароля уже использована");
        }
    }
}