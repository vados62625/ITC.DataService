using AutoMapper;
using FluentValidation;
using ITC.Authorization.CQRS.InviteUsers.Dto;
using ITC.Authorization.Enums;
using ITC.Authorization.Storage;
using ITC.CQRS.Base;
using ITC.CQRS.Base.Add;
using MediatR;

namespace ITC.Authorization.CQRS.InviteUsers;

public class Add : AddEntityCommandBase, IRequest
{
    public Guid? EmployeeId { get; set; }
    public Guid? UserId { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? MiddleName { get; set; }

    public Gender Gender { get; set; }

    public string Email { get; set; }

    public string? Position { get; set; }
    
    public string Role { get; set; }

    public string InviteToken { get; set; }

    public class Handler : AddEntityCommandHandlerBase<ServiceDbContext, UserApiRequestWrapper<Add, InvitedUserDto>, Add, InvitedUser, InvitedUserDto>
    {
        public Handler(IMapper mapper, ServiceDbContext db, IValidator<UserApiRequestWrapper<Add, InvitedUserDto>> validator) : base(mapper, db, validator)
        {
        }
    }

    public class Validator : AddEntityCommandValidatorBase<ServiceDbContext, UserApiRequestWrapper<Add, InvitedUserDto>, Add, InvitedUserDto>
    {
        public Validator(ServiceDbContext db) : base(db)
        {
        }
    }
}
