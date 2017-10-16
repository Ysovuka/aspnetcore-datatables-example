using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.DataTables.Web.Models.DataTables
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> query, IEnumerable<Column> columns, string value)
            where T : class
        {
            Expression<Func<T, bool>> containsExpression = null;

            if (!string.IsNullOrEmpty(value))
            {
                containsExpression = query.Contains(columns, value: value, bitwiseOr: true);
            }

            containsExpression = query.Contains(columns, expression: containsExpression);

            if (containsExpression != null)
                return query.Where(containsExpression.Compile()).ToList();

            return query;
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> query, IEnumerable<Column> columns, string value)
            where T : class
        {
            Expression<Func<T, bool>> containsExpression = null;

            if (!string.IsNullOrEmpty(value))
            {
                containsExpression = query.Contains(columns, value: value, bitwiseOr: true);
            }

            containsExpression = query.Contains(columns, expression: containsExpression);

            if (containsExpression != null)
                return query.Where(containsExpression);

            return query;
        }

        public static IOrderedEnumerable<T> Sort<T>(this IEnumerable<T> query, IEnumerable<Order> orders, IEnumerable<Column> columns)
            where T : class
        {
            IOrderedEnumerable<T> orderedFilteredCompanies = null;
            var orderCount = 0;
            foreach (var order in orders)
            {
                var column = columns.ToList()[order.column];
                var dir = order.dir;
                if (column != null)
                {
                    var propertyName = column.name;
                    if (orderCount == 0)
                        orderedFilteredCompanies = query.OrderBy(propertyName, dir);
                    else
                        orderedFilteredCompanies = orderedFilteredCompanies.ThenBy(propertyName, dir);
                }

                orderCount++;
            }

            return orderedFilteredCompanies;
        }

        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> query, IEnumerable<Order> orders, IEnumerable<Column> columns)
            where T : class
        {
            IOrderedQueryable<T> orderedFilteredCompanies = null;
            var orderCount = 0;
            foreach (var order in orders)
            {
                var column = columns.ToList()[order.column];
                var dir = order.dir;
                if (column != null)
                {
                    var propertyName = column.name;
                    if (orderCount == 0)
                        orderedFilteredCompanies = query.OrderBy(propertyName, dir);
                    else
                        orderedFilteredCompanies = orderedFilteredCompanies.ThenBy(propertyName, dir);
                }

                orderCount++;
            }

            return orderedFilteredCompanies;
        }

        public static Expression<Func<T, bool>> Contains<T>(this IEnumerable<T> query,
                IEnumerable<Column> columns,
                string value = null,
                Expression<Func<T, bool>> expression = default(Expression<Func<T, bool>>),
                bool bitwiseOr = false)
            where T : class
        {
            foreach (var column in columns)
            {
                var searchValue = (!string.IsNullOrEmpty(value)) ? value : column.search.value;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (expression == null)
                        expression = query.Contains(column.name, searchValue);
                    else
                        if (!bitwiseOr)
                            expression = expression.And(query.Contains(column.name, searchValue));
                        else
                            expression = expression.Or(query.Contains(column.name, searchValue));
                }
            }

            return expression;
        }

        public static Expression<Func<T, bool>> Contains<T>(this IQueryable<T> query,
                IEnumerable<Column> columns,
                string value = null,
                Expression<Func<T, bool>> expression = default(Expression<Func<T, bool>>),
                bool bitwiseOr = false)
            where T : class
        {
            foreach (var column in columns)
            {
                var searchValue = (!string.IsNullOrEmpty(value)) ? value : column.search.value;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (expression == null)
                        expression = query.Contains(column.name, searchValue);
                    else
                        if (!bitwiseOr)
                        expression = expression.And(query.Contains(column.name, searchValue));
                    else
                        expression = expression.Or(query.Contains(column.name, searchValue));
                }
            }

            return expression;
        }

        private static Expression<Func<T, bool>> Contains<T>(this IEnumerable<T> query, string propertyName, string value)
            where T: class
        {
            return t => GetPropertyValue(propertyName, t).ToString().ToLower().Contains(value.ToLower());
        }

        private static Expression<Func<T, bool>> Contains<T>(this IQueryable<T> query, string propertyName, string value)
            where T : class
        {
            return t => GetPropertyValue(propertyName, t).ToString().ToLower().Contains(value.ToLower());
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> query, string propertyName, string direction = "asc")
            where T : class
        {
            return (direction.ToLower() == "asc") ?
                query.OrderBy(GetPropertyFunc<T>(propertyName)) :
                query.OrderByDescending(GetPropertyFunc<T>(propertyName));
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, string direction = "asc")
            where T : class
        {
            return (direction.ToLower() == "asc") ?
                query.OrderBy(GetPropertyExpression<T>(propertyName)) :
                query.OrderByDescending(GetPropertyExpression<T>(propertyName));
        }

        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> query, string propertyName, string direction = "asc")
            where T : class
        {
            return (direction.ToLower() == "asc") ?
                query.ThenBy(GetPropertyFunc<T>(propertyName)) :
                query.ThenByDescending(GetPropertyFunc<T>(propertyName));
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string propertyName, string direction = "asc")
            where T : class
        {
            return (direction.ToLower() == "asc") ?
                query.ThenBy(GetPropertyExpression<T>(propertyName)) :
                query.ThenByDescending(GetPropertyExpression<T>(propertyName));
        }

        private static Expression<Func<T, object>> GetPropertyExpression<T>(string propertyName)
            where T : class
        {
            return t => typeof(T).GetProperty(propertyName).GetMethod.Invoke(t, new object[] { });
        }

        private static Func<T, object> GetPropertyFunc<T>(string propertyName)
            where T : class
        {
            return t => typeof(T).GetProperty(propertyName).GetMethod.Invoke(t, new object[] { });
        }

        private static object GetPropertyValue<T>(string propertyName, T obj)
        {
            return typeof(T).GetProperty(propertyName).GetValue(obj);
        }
    }
}
