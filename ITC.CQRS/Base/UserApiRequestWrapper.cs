using ITC.CQRS.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ITC.CQRS.Base;

/// <summary>
/// Базовый класс обертки для API запроса авторизованного пользователя
/// </summary>
/// <typeparam name="TRequest">Тип тела API запроса</typeparam>
/// <typeparam name="TResponse">Тип результата</typeparam>

public class UserApiRequestWrapper<TRequest, TResponse>
    : IRequest<TResponse> where TRequest : class
{
    public TRequest Request { get; }
    /// <summary>
    /// Контекст авторизованного пользователя
    /// </summary>
    public UserContext UserContext { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request">Тело API запроса</param>
    /// <param name="httpContext">Контекст выполнения API запроса</param>
    public UserApiRequestWrapper(TRequest request, HttpContext httpContext)
    {
        Request = request;
        UserContext = httpContext.User.GetUserContext();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request">Тело API запроса</param>
    /// <param name="userContext">Контекст пользователя</param>
    public UserApiRequestWrapper(TRequest request, UserContext userContext)
    {
        Request = request;
        UserContext = userContext;
    }
}
/// <summary>
/// Базовый класс обертки для API запроса авторизованного пользователя, без возвращаемого результата
/// </summary>
/// <typeparam name="TRequest">Тип тела API запроса</typeparam>
public class UserApiRequestWrapper<TRequest>
    : IRequest where TRequest : class
{
    public TRequest Request { get; }
    /// <summary>
    /// Контекст авторизованного пользователя
    /// </summary>
    public UserContext UserContext { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request">Тело API запроса</param>
    /// <param name="httpContext">Контекст выполнения API запроса</param>
    public UserApiRequestWrapper(TRequest request, HttpContext httpContext)
    {
        Request = request;
        UserContext = httpContext.User.GetUserContext();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request">Тело API запроса</param>
    /// <param name="userContext">Контекст пользователя</param>
    public UserApiRequestWrapper(TRequest request, UserContext userContext)
    {
        Request = request;
        UserContext = userContext;
    }
}