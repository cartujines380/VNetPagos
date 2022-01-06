using System;
using System.Linq;
using System.Linq.Expressions;

namespace VisaNet.Common.FrameworkExtensions
{
    public static class SortBy
    {
        public static IQueryable<T> OrderByStringProperty<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> OrderByStringPropertyDescending<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

    }
}