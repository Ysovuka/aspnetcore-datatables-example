using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        private static Expression<Func<T, bool>> Contains<T>(this IEnumerable<T> query, string propertyName, string value)
            where T: class
        {
            try
            {
                ConstantExpression searchArgument = Expression.Constant(value);
                ParameterExpression param = Expression.Parameter(typeof(T));

                Expression property = Expression.Property(param, propertyName);

                MethodInfo toStringMethod = typeof(T).GetMethod("ToString");

                property = Expression.Call(property, toStringMethod);

                MethodInfo containsMethod = typeof(string).GetMethod("Contains");

                MethodCallExpression fieldExpression = Expression.Call(property, containsMethod, searchArgument);

                return Expression.Lambda<Func<T, bool>>(fieldExpression, param);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return default(Expression<Func<T, bool>>);
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> query, string propertyName, string direction = "asc")
            where T : class
        {
            return (direction.ToLower() == "asc") ?
                query.OrderBy(GetPropertyFunc<T>(propertyName)) :
                query.OrderByDescending(GetPropertyFunc<T>(propertyName));
        }

        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> query, string propertyName, string direction = "asc")
            where T : class
        {
            return (direction.ToLower() == "asc") ?
                query.ThenBy(GetPropertyFunc<T>(propertyName)) :
                query.ThenByDescending(GetPropertyFunc<T>(propertyName));
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

        public static Expression<Func<T, bool>> Contains<T>(this IQueryable<T> query,
                IEnumerable<Column> columns,
                string value = null,
                Expression<Func<T, bool>> expression = default(Expression<Func<T, bool>>),
                bool bitwiseOr = false)
            where T : class
        {
            foreach (var column in columns)
            {
                if (!string.IsNullOrEmpty(column.name))
                {
                    var columnNames = column.name.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var columnName in columnNames)
                    {
                        var propertyName = columnName.Replace(';', ' ').Replace(',', ' ').Trim();
                        var searchValue = (!string.IsNullOrEmpty(value)) ? value : column.search.value;
                        var isValueType = typeof(T).GetProperty(propertyName).PropertyType.IsValueType;

                        if (!string.IsNullOrEmpty(searchValue) && !isValueType && column.searchable)
                        {
                            if (expression == null)
                                expression = query.Contains(propertyName, searchValue);
                            else
                                if (!bitwiseOr)
                                expression = expression.And(query.Contains(propertyName, searchValue));
                            else
                                expression = expression.Or(query.Contains(propertyName, searchValue));
                        }
                    }
                }
            }
            return expression;
        }

        private static Expression<Func<T, bool>> Contains<T>(this IQueryable<T> query, string propertyName, string value)
            where T : class
        {
            try
            {
                ConstantExpression searchArgument = Expression.Constant(value);
                ParameterExpression param = Expression.Parameter(typeof(T));

                Expression property = Expression.Property(param, propertyName);

                MethodInfo toStringMethod = typeof(T).GetMethod("ToString");

                property = Expression.Call(property, toStringMethod);

                MethodInfo containsMethod = typeof(string).GetMethod("Contains");

                MethodCallExpression fieldExpression = Expression.Call(property, containsMethod, searchArgument);

                return Expression.Lambda<Func<T, bool>>(fieldExpression, param);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return default(Expression<Func<T, bool>>);
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, IEnumerable<Order> orders, IEnumerable<Column> columns)
            where T : class
        {
            IQueryable<T> orderedFilteredCompanies = null;
            var orderCount = 0;
            foreach (var order in orders)
            {
                var column = columns.ToList()[order.column];
                var dir = order.dir;
                if (column != null)
                {
                    var columnNames = column.name.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var columnName in columnNames)
                    {
                        var propertyName = columnName.Replace(';', ' ').Replace(',', ' ').Trim();
                        if (!string.IsNullOrEmpty(propertyName))
                        {
                            if (orderCount == 0)
                                orderedFilteredCompanies = query.OrderBy(propertyName, dir);
                            else
                                orderedFilteredCompanies = orderedFilteredCompanies.ThenBy(propertyName, dir);
                        }

                        orderCount++;
                    }
                }
            }

            return orderedFilteredCompanies ?? query;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, string direction = "asc")
            where T : class
        {
            var param = Expression.Parameter(typeof(T));

            var memberAccess = Expression.PropertyOrField(param, propertyName);

            var keySelector = Expression.Lambda(memberAccess, param);

            var orderBy = Expression.Call(
                typeof(Queryable),
                direction == "asc" ? "OrderBy" : "OrderByDescending",
                new Type[] { typeof(T), memberAccess.Type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> query, string propertyName, string direction = "asc")
            where T : class
        {
            var param = Expression.Parameter(typeof(T));

            var memberAccess = Expression.PropertyOrField(param, propertyName);

            var keySelector = Expression.Lambda(memberAccess, param);

            var orderBy = Expression.Call(
                typeof(Queryable),
                direction == "asc" ? "ThenBy" : "ThenByDescending",
                new Type[] { typeof(T), memberAccess.Type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
