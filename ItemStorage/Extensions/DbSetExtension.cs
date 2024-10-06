using System.Linq.Expressions;
using ItemStorage.Data;
using ItemStorage.Exceptions;
using ItemStorage.Util;
using Microsoft.EntityFrameworkCore;

namespace ItemStorage.Extensions;

public static class DbSetExtension
{
    public static IQueryable<T> ApplyFilter<T>(this DbSet<T> dbSet, IEnumerable<DataFilter>? filters) where T : class
    {
        var query = dbSet.AsQueryable();

        var properties = typeof(T).GetProperties().ToList();

        if (filters == null)
        {
            return query;
        }
        
        foreach (var filter in filters)
        {
            var property = properties.FirstOrDefault(p =>
                p.Name.Equals(filter.PropertyName, StringComparison.CurrentCultureIgnoreCase));

            if (property == null)
                throw new FilterException($"Property with name {filter.PropertyName} doesn't exist");

            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, property.Name);

            if (!JsonElementHelper.TryConvert(filter.Value, member.Type, out var value))
                throw new FilterException(
                    $"Failed to convert filter {filter.PropertyName} value to type {member.Type.Name}");

            Expression constant = Expression.Constant(value);
            Expression body;
            Expression<Func<T, bool>> expression;

            switch (filter.DataFilterType)
            {
                case DataFilterType.Equals:
                    body = Expression.Equal(member, constant);
                    expression = Expression.Lambda<Func<T, bool>>(body, parameter);
                    query = query.Where(expression);
                    break;
                case DataFilterType.Contains:
                    if (member.Type != typeof(string))
                        throw new FilterException("Type of property must be string");

                    var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
                    body = Expression.Call(member, contains, constant);
                    expression = Expression.Lambda<Func<T, bool>>(body, parameter);
                    query = query.Where(expression);
                    break;
                default:
                    throw new FilterException("Type of filter does not exist");
            }
        }

        return query;
    }
}