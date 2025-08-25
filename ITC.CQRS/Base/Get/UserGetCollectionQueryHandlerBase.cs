using AutoMapper;
using AutoMapper.QueryableExtensions;
using ITC.Domain.Dto;
using ITC.Domain.Models;
using ITC.Storage.Extensions;
using ITC.Storage.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.CQRS.Base.Get;

/// <summary>
/// Базовый обработчик запроса получения авторизованным пользователем коллекции из БД
/// </summary>
/// <typeparam name="TDbContext">Тип контекста БД</typeparam>
/// <typeparam name="TEntity">Тип сущности БД</typeparam>
/// <typeparam name="TResponse">Тип ответа (DTO)</typeparam>
/// <typeparam name="TRequest">Тип тела API запроса</typeparam>
public abstract class UserGetCollectionQueryHandlerBase <TDbContext, TRequest, TEntity, TResponse> 
    : IRequestHandler<UserApiGetCollectionRequestWrapper<TRequest, TResponse>, IPageableCollection<TResponse>>

    where TRequest : class, IEntityQuery
    where TEntity : class, IEntity
    where TResponse :class, IEntityDto
    where TDbContext : DbContext
{
    protected IMapper Mapper { get; }
    protected TDbContext Db { get; }

    protected UserGetCollectionQueryHandlerBase(IMapper mapper, TDbContext db)
    {
        Mapper = mapper;
        Db = db;
    }

    public async Task<IPageableCollection<TResponse>> Handle(UserApiGetCollectionRequestWrapper<TRequest, TResponse> requestWrapper, CancellationToken cancellationToken)
    {
        var query = Db.Set<TEntity>()
            .AsNoTracking();

        query = query.Sort(requestWrapper.Request);

        query = await PreRequestAction(requestWrapper, query, cancellationToken);

        query = query.FilterBy(requestWrapper.Request);
        
        var dtoQuery = Project(requestWrapper, query);

        dtoQuery = await PostProjectAction(requestWrapper, dtoQuery, cancellationToken);

        IPageableCollection<TResponse> collection = 
            await dtoQuery.ToPageableCollection(requestWrapper.Request, cancellationToken);

        collection = await PostRequestAction(requestWrapper, collection, cancellationToken);

        return collection;
    }

    /// <summary>
    /// Для переопределения действия перед выполнением запроса к БД
    /// </summary>
    /// <param name="request"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual ValueTask<IQueryable<TEntity>> PreRequestAction(
        UserApiGetCollectionRequestWrapper<TRequest, TResponse> request, 
        IQueryable<TEntity> query,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(query);
    }

    /// <summary>
    /// Для переопределения проекции сущности (Automapper)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    protected virtual IQueryable<TResponse> Project(UserApiGetCollectionRequestWrapper<TRequest, TResponse> request, IQueryable<TEntity> query)
    {
        return query.ProjectTo<TResponse>(Mapper.ConfigurationProvider);
    }

    /// <summary>
    ///  Для переопределения действия после проекции сущности (Automapper)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual ValueTask<IQueryable<TResponse>> PostProjectAction(
        UserApiGetCollectionRequestWrapper<TRequest, TResponse> request,
        IQueryable<TResponse> query,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(query);
    }

    /// <summary>
    /// Для переопределения действия после выполнения запроса к БД
    /// </summary>
    /// <param name="request"></param>
    /// <param name="collection"></param>
    /// <param name="cancellationToken"></param>    
    /// <returns></returns>
    protected virtual ValueTask<IPageableCollection<TResponse>> PostRequestAction(
        UserApiGetCollectionRequestWrapper<TRequest, TResponse> request,
        IPageableCollection<TResponse> collection,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(collection);
    }
}