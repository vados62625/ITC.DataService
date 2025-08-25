using FluentValidation;
using ITC.Domain.Dto;

namespace ITC.CQRS.Base.Add;

public abstract class AddEntityCommandValidatorBase<TDbContext, TWrapper, TCommand, TResponse> : AbstractValidator<TWrapper> 
    where TWrapper : UserApiRequestWrapper<TCommand, TResponse>
    where TCommand : class, IAddEntityCommandBase
    where TResponse : IEntityDto
{
    protected AddEntityCommandValidatorBase(TDbContext db)
    {
    }
}