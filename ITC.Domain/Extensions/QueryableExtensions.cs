using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using ITC.Domain.Attributes;
using ITC.Domain.CQRS.Base;
using ITC.Domain.Dto;
using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ITC.Domain.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> NotDeleted<T>(this IQueryable<T> query) where T : EntityBase
    {
        return query.Where(c => c.DeletedAt == null);
    }
    public static IQueryable<TModel> FilterBy<TModel, TFilter>(this IQueryable<TModel> query, TFilter filter)
        where TModel : class
        where TFilter : class
    {
        var suggestedProperties = new List<PropertyInfo>();
        foreach (var propertyInfo in filter.GetType().GetProperties())
        {
            // Ignore prop by attribute
            var attributes = propertyInfo.GetCustomAttributes(false);
            if (attributes.Any(a => a.GetType() == typeof(FilterIgnoreAttribute)))
                continue;

            var modelProperty = typeof(TModel).GetProperty(propertyInfo.Name);

            var value = propertyInfo.GetValue(filter, null);

            if (value == null || modelProperty == null)
                continue;

            var param = Expression.Parameter(typeof(TModel));
            var getProperty = Expression.MakeMemberAccess(param, modelProperty);
            var propertyValue = Expression.Constant(value);
            var convertedValue = Expression.Convert(propertyValue, modelProperty.PropertyType);

            Expression expr = Expression.Equal(getProperty, convertedValue);

            if (attributes.Any())
                expr = expr.ByAttributes(attributes, getProperty, convertedValue);

            var predicate = Expression.Lambda<Func<TModel, bool>>(expr, param);

            query = query.Where(predicate);
        }
        return query;
    }
    private static Expression ByAttributes(this Expression exp, object[] attributes, Expression getProperty,
        Expression constant)
    {
        var isCaseInsensitiveSearch = attributes.Any(a => a.GetType() == typeof(FilterCaseInsensitiveAttribute));
        var isSubstringSearch = attributes.Any(a => a.GetType() == typeof(FilterSubstringSearchAttribute));

        var toLowerMethodInfo = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes) ??
                                throw new InvalidOperationException();

        var containsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) }) ??
                                 throw new InvalidOperationException();

        var lowerPropertyValue = Expression.Call(getProperty, toLowerMethodInfo);
        var lowerConstantValue = Expression.Call(constant, toLowerMethodInfo);

        if (isCaseInsensitiveSearch && isSubstringSearch)
        {
            exp = Expression.Call(lowerPropertyValue, containsMethodInfo, lowerConstantValue);
            return exp;
        }

        if (isCaseInsensitiveSearch)
            exp = Expression.Equal(lowerPropertyValue, lowerConstantValue);

        if (isSubstringSearch)
            exp = Expression.Call(getProperty, containsMethodInfo, constant);

        return exp;
    }
    public static IRuleBuilderOptions<T, Guid> EntityMustExistAsync<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        DbSet<TEntity> dbSet)
        where TEntity : EntityBase
    {
        return ruleBuilder.MustAsync(async (id, ct) =>
                await dbSet.AnyAsync(e => e.Id == id, ct))
            .WithMessage((query, id)=>$"Entity {typeof(TEntity).Name} with id {id} not found");
    }

    public static IRuleBuilderOptions<T, Guid?> EntityMustExistAsync<T, TEntity>(
        this IRuleBuilder<T, Guid?> ruleBuilder,
        DbSet<TEntity> dbSet)
        where TEntity : EntityBase
    {
        return ruleBuilder.MustAsync(async (id, ct) =>
            {
                if (id == null)
                    return true;
                return await dbSet.AnyAsync(e => e.Id == id, ct);
            })
            .WithMessage((query, id) => $"Entity {typeof(TEntity).Name} with id {id} not found");
    }
    
    public static IQueryable<TModel> Pagination<TModel>(this IQueryable<TModel> query, IPagingQuery pagingQuery)
    {
        switch (pagingQuery.Page)
        {
            case <= 0:
                return query;
        }

        switch (pagingQuery.ItemsCount)
        {
            case <= 0:
                return query;
        }

        return query
            .Skip((pagingQuery.Page - 1) * pagingQuery.ItemsCount)
            .Take(pagingQuery.ItemsCount);
    }

    public static async Task<PageableCollection<TModel>> ToPageableCollectionAsync<TModel>(this IQueryable<TModel> query, IPagingQuery pagingQuery, CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        
        var entities = await query
            .Pagination(pagingQuery)
            .ToArrayAsync(cancellationToken);

        return new PageableCollection<TModel>(entities, totalCount);
    }
}