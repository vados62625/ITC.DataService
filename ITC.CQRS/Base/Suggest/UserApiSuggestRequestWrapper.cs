using Microsoft.AspNetCore.Http;

namespace ITC.CQRS.Base.Suggest;

public class UserApiSuggestRequestWrapper<TRequest> : UserApiRequestWrapper<TRequest, SuggestResultDto[]>
    where TRequest : class, ISuggestRequestBase
{
    public UserApiSuggestRequestWrapper(TRequest request, HttpContext httpContext) : base(request, httpContext)
    {
    }
}