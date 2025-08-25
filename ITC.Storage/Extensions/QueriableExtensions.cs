using System.Linq.Expressions;
using System.Reflection;
using ITC.Domain.Dto;
using ITC.Domain.Models;
using ITC.Storage.Attributes;
using ITC.Storage.Query;
using Microsoft.EntityFrameworkCore;

namespace ITC.Storage.Extensions;

public static class QueriableExtensions
{
    public static async Task<IPageableCollection<TModel>> ToPageableCollection<TModel>(this IQueryable<TModel> query,
        IPagingQuery pagingQuery,
        CancellationToken cancellationToken,
        bool isNeedTotalCount = true)
    {
        var totalCount = isNeedTotalCount ? await query.CountAsync(cancellationToken) : 0;
        var entities = await query
            .Pagination(pagingQuery)
            .ToArrayAsync(cancellationToken);

        return new PageableCollection<TModel>(entities, totalCount);
    }
    public static IQueryable<TModel> Pagination<TModel>(this IQueryable<TModel> query, IPagingQuery pagingQuery)
    {
        if (pagingQuery.Page is null or 0)
            pagingQuery.Page = 1;

        if (pagingQuery.ItemsOnPage is null or 0)
            pagingQuery.ItemsOnPage = 100;

        return query
            .Skip((pagingQuery.Page.Value - 1) * pagingQuery.ItemsOnPage.Value)
            .Take(pagingQuery.ItemsOnPage.Value);
    }

    public static IQueryable<TModel> Sort<TModel>(this IQueryable<TModel> query, IPagingQuery pagingQuery) 
        where TModel : IEntity
    {
        var modelType = typeof(TModel);

        var sortProps = pagingQuery.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(SortDirection?));

        foreach (var sortProp in sortProps)
        {
            var sortKey = sortProp.Name;
            var direction = (SortDirection?)sortProp.GetValue(pagingQuery);
            if (direction == null) continue;

            var propertyName = sortKey.Replace("SortDirection", "");

            var sortableAttr = sortProp.GetCustomAttribute<SortAttribute>();
            if (sortableAttr == null) continue;

            var path = sortableAttr.SortPath ?? propertyName;

            var expression = BuildPropertyExpression<TModel>(path);

            query = query.ApplyOrderBy(expression, direction.Value);
        }

        // если ничего не отсортировали
        if (query is not IOrderedQueryable<TModel> && modelType.GetProperty("CreatedAt") != null)
        {
            var fallback = BuildPropertyExpression<TModel>("CreatedAt");
            query = query.ApplyOrderBy(fallback, SortDirection.Descending);
        }

        return query;
    }
    
    public static Expression<Func<TModel, object>> BuildPropertyExpression<TModel>(string propertyPath)
    {
        var parameter = Expression.Parameter(typeof(TModel), "x");

        Expression body = parameter;
        foreach (var member in propertyPath.Split('.'))
        {
            body = Expression.PropertyOrField(body, member);
        }

        var converted = Expression.Convert(body, typeof(object));
        return Expression.Lambda<Func<TModel, object>>(converted, parameter);
    }
    
    public static IQueryable<TModel> ApplyOrderBy<TModel>(
        this IQueryable<TModel> query,
        Expression<Func<TModel, object>> lambda,
        SortDirection direction)
    {
        if (query.Expression.Type == typeof(IOrderedQueryable<TModel>))
        {
            // уже есть сортировка — добавляем ThenBy
            return direction switch
            {
                SortDirection.Ascending => ((IOrderedQueryable<TModel>)query).ThenBy(lambda),
                SortDirection.Descending => ((IOrderedQueryable<TModel>)query).ThenByDescending(lambda),
                _ => query
            };
        }

        // ещё нет сортировки — применяем OrderBy
        return direction switch
        {
            SortDirection.Ascending => query.OrderBy(lambda),
            SortDirection.Descending => query.OrderByDescending(lambda),
            _ => query
        };
    }

    /// <summary>
    /// Производит фильтрацию по атрибутам свойств <see cref="Attributes"/>
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    /// <param name="query"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IQueryable<TModel> FilterBy<TModel, TFilter>(this IQueryable<TModel> query, TFilter filter)
        where TModel : class
        where TFilter : class
    {
        var suggestedProperties = filter.GetType().GetProperties();

        foreach (var propertyInfo in suggestedProperties)
        {
            switch (propertyInfo.Name)
            {
                // Ignore pagination props
                case nameof(IEntityQuery.Page):
                case nameof(IEntityQuery.ItemsOnPage):
                case nameof(IEntityQuery.CreatedAtSortDirection):
                case nameof(IEntityQuery.UpdatetAtSortDirection):
                    continue;
            }

            // Ignore prop by attribute
            var attributes = propertyInfo.GetCustomAttributes(false);
            if (attributes.Any(a => a.GetType() == typeof(FilterIgnoreAttribute)))
                continue;

            var modelProperty = propertyInfo.Name switch
            {
                nameof(IEntityQuery.CreatedAtFrom) => typeof(TModel).GetProperty(nameof(EntityBase.CreatedAt)),
                nameof(IEntityQuery.CreatedAtTo) => typeof(TModel).GetProperty(nameof(EntityBase.CreatedAt)),
                nameof(IEntityQuery.Timestamp) => typeof(TModel).GetProperty(nameof(EntityBase.Timestamp)),
                _ => typeof(TModel).GetProperty(propertyInfo.Name)
            };

            var value = propertyInfo.GetValue(filter, null);

            if (value == null || modelProperty == null)
                continue;

            var param = Expression.Parameter(typeof(TModel));
            var getProperty = Expression.MakeMemberAccess(param, modelProperty);
            var propertyValue = Expression.Constant(value);
            var convertedValue = Expression.Convert(propertyValue, modelProperty.PropertyType);

            Expression expr = Expression.Equal(getProperty, convertedValue);

            if (propertyInfo.Name == nameof(IEntityQuery.CreatedAtFrom))
                expr = Expression.GreaterThanOrEqual(getProperty, propertyValue);

            if (propertyInfo.Name == nameof(IEntityQuery.CreatedAtTo))
                expr = Expression.LessThanOrEqual(getProperty, propertyValue);

            if (propertyInfo.Name == nameof(IEntityQuery.Timestamp))
                expr = Expression.GreaterThan(getProperty, propertyValue);

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

        var toLowerMethodInfo = typeof(string).GetMethod("ToLower", Type.EmptyTypes) ??
                                throw new InvalidOperationException();

        var startWithMethodInfo = typeof(string).GetMethod("StartsWith", new[] { typeof(string) }) ??
                                 throw new InvalidOperationException();

        var lowerPropertyValue = Expression.Call(getProperty, toLowerMethodInfo);
        var lowerConstantValue = Expression.Call(constant, toLowerMethodInfo);

        if (isCaseInsensitiveSearch && isSubstringSearch)
        {
            exp = Expression.Call(lowerPropertyValue, startWithMethodInfo, lowerConstantValue);
            return exp;
        }

        if (isCaseInsensitiveSearch)
            exp = Expression.Equal(lowerPropertyValue, lowerConstantValue);

        if (isSubstringSearch)
            exp = Expression.Call(getProperty, startWithMethodInfo, constant);

        return exp;
    }
}