using AutoMapper;
using FluentValidation;
using ITC.Authorization.Storage;
using ITC.CQRS.Base;
using ITC.CQRS.Base.Delete;

namespace ITC.Authorization.CQRS.PasswordResetUsers;

public class Delete : DeleteEntityCommandBase
{
    public class Handler(IMapper mapper, ServiceDbContext db, IValidator<UserApiRequestWrapper<Delete>> validator)
        : DeleteEntityCommandHandlerBase<ServiceDbContext, UserApiRequestWrapper<Delete>, Delete, PasswordResetUser>
            (mapper, db, validator);

    public class Validator(ServiceDbContext db) : DeleteEntityCommandValidatorBase<ServiceDbContext,
        UserApiRequestWrapper<Delete>, Delete, PasswordResetUser>(db);
}