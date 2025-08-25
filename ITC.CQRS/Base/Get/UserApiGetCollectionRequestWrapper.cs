using ITC.Domain.Dto;
using ITC.Storage.Query;
using Microsoft.AspNetCore.Http;

namespace ITC.CQRS.Base.Get;

/// <summary>
/// Базовый клас обертки запроса API получения авторизованным пользователем коллекции из БД
/// </summary>
/// <typeparam name="TRequest">Тип тела API запроса</typeparam>
/// <typeparam name="TResponse">Тип результата</typeparam>

public class UserApiGetCollectionRequestWrapper<TRequest, TResponse>
    : UserApiRequestWrapper<TRequest, IPageableCollection<TResponse>>
    where TResponse : IEntityDto
    where TRequest : class, IEntityQuery
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request">Тело API запроса</param>
    /// <param name="httpContext">Контекст выполнения API запроса</param>
    public UserApiGetCollectionRequestWrapper(TRequest request, HttpContext httpContext) : base(request, httpContext)
    {
        
    }
}