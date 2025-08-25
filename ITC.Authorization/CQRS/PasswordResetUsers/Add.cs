using AutoMapper;
using FluentValidation;
using ITC.Authorization.CQRS.PasswordResetUsers.Dto;
using ITC.Authorization.Storage;
using ITC.CQRS.Base;
using ITC.CQRS.Base.Add;
using MediatR;

namespace ITC.Authorization.CQRS.PasswordResetUsers;

public class Add : AddEntityCommandBase, IRequest
{
    public Guid UserId { get; set; }
    public string PasswordResetToken { get; set; }

    public class Handler : AddEntityCommandHandlerBase<ServiceDbContext, UserApiRequestWrapper<Add, PasswordResetUserDto>, Add, PasswordResetUser, PasswordResetUserDto>
    {
        public Handler(IMapper mapper, ServiceDbContext db, IValidator<UserApiRequestWrapper<Add, PasswordResetUserDto>> validator) : base(mapper, db, validator)
        {
        }
    }

    public class Validator : AddEntityCommandValidatorBase<ServiceDbContext, UserApiRequestWrapper<Add, PasswordResetUserDto>, Add, PasswordResetUserDto>
    {
        public Validator(ServiceDbContext db) : base(db)
        {
        }
    }
}