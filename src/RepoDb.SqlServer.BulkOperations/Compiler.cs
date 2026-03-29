using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Exceptions;

namespace RepoDb.SqlServer.BulkOperations;

/// <summary>
/// An internal compiler class used to compile necessary expressions that is needed to enhance the code execution.
/// </summary>
internal static class Compiler
{

    #region GetParameterizedMethodFunc

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="methodName"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static Func<TEntity, object[], TResult>? GetParameterizedMethodFunc<TEntity, TResult>(string methodName,
        Type[] types)
        where TEntity : class =>
        ParameterizedMethodFuncCache<TEntity, TResult>.GetFunc(methodName, types);

    private static class ParameterizedMethodFuncCache<TEntity, TResult>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<int, Func<TEntity, object?[], TResult>?> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Func<TEntity, object?[], TResult>? GetFunc(string methodName,
            Type[] types)
        {
            var key = methodName.GetHashCode() + types.Sum(e => e.GetHashCode());

            return cache.GetOrAdd(key, _ =>
            {
                var typeOfEntity = typeof(TEntity);
                var method = typeOfEntity.GetMethod(methodName, types);
                if (method is not null)
                {
                    var entity = Expression.Parameter(typeOfEntity, "entity");
                    var arguments = Expression.Parameter(typeof(object[]), "arguments");
                    var parameters = new List<Expression>();
                    for (var index = 0; index < types.Length; index++)
                    {
                        parameters.Add(Expression.Convert(Expression.ArrayIndex(arguments, Expression.Constant(index)), types[index]));
                    }
                    var body = Expression.Convert(Expression.Call(entity, method, parameters), typeof(TResult));
                    return Expression
                        .Lambda<Func<TEntity, object?[], TResult>>(body, entity, arguments)
                        .Compile();
                }
                else
                    return null;
            });
        }
    }

    #endregion

    #region GetParameterizedVoidMethodFunc

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="methodName"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static Action<TEntity, object?[]>? GetParameterizedVoidMethodFunc<TEntity>(string methodName,
        Type[] types)
        where TEntity : class =>
        ParameterizedVoidMethodFuncCache<TEntity>.GetFunc(methodName, types);

    private static class ParameterizedVoidMethodFuncCache<TEntity>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<int, Action<TEntity, object?[]>?> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Action<TEntity, object?[]>? GetFunc(string methodName,
            Type[] types)
        {
            var key = methodName.GetHashCode() + types.Sum(e => e.GetHashCode());

            return cache.GetOrAdd(key, (_) =>
            {
                var typeOfEntity = typeof(TEntity);
                var method = typeOfEntity.GetMethod(methodName, types);

                if (method is not null)
                {
                    var entity = Expression.Parameter(typeOfEntity, "entity");
                    var arguments = Expression.Parameter(typeof(object[]), "arguments");
                    var parameters = new List<Expression>();

                    for (var index = 0; index < types.Length; index++)
                    {
                        parameters.Add(Expression.Convert(Expression.ArrayIndex(arguments, Expression.Constant(index)), types[index]));
                    }

                    var body = Expression.Call(entity, method, parameters);

                    return Expression
                        .Lambda<Action<TEntity, object?[]>>(body, entity, arguments)
                        .Compile();
                }
                else
                    return null;
            });
        }
    }

    #endregion

    #region GetPropertyGetterFunc

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Func<TEntity, TResult> GetPropertyGetterFunc<TEntity, TResult>(string propertyName)
        where TEntity : class =>
        PropertyGetterFuncCache<TEntity, TResult>.GetFunc(PropertyCache.Get<TEntity>(propertyName) ?? throw new PropertyNotFoundException($"Property {propertyName} not found"));

    private static class PropertyGetterFuncCache<TEntity, TResult>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<int, Func<TEntity, TResult>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        public static Func<TEntity, TResult> GetFunc(ClassProperty classProperty)
        {
            if (cache.TryGetValue(classProperty.GetHashCode(), out var func) == false)
            {
                var typeOfEntity = typeof(TEntity);
                var entity = Expression.Parameter(typeOfEntity, "entity");
                var body = Expression.Convert(Expression.Call(entity, classProperty.PropertyInfo.GetMethod!), typeof(TResult));

                func = Expression
                    .Lambda<Func<TEntity, TResult>>(body, entity)
                    .Compile();

                cache.TryAdd(classProperty.GetHashCode(), func);
            }
            return func;
        }
    }

    #endregion

    #region GetPropertySetterFunc

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Action<TEntity, object?>? GetPropertySetterFunc<TEntity>(string propertyName)
        where TEntity : class =>
        PropertySetterFuncCache<TEntity>.GetFunc(PropertyCache.Get<TEntity>(propertyName, true));

    private static class PropertySetterFuncCache<TEntity>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<int, Action<TEntity, object?>?> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        public static Action<TEntity, object?>? GetFunc(ClassProperty? classProperty)
        {
            if (classProperty is null)
            {
                return null;
            }

            return cache.GetOrAdd(classProperty.GetHashCode(), (_) =>
            {
                if (classProperty is not null)
                {
                    var entity = Expression.Parameter(typeof(TEntity), "entity");
                    var value = Expression.Parameter(typeof(object), "value");
                    var converted = Expression.Convert(value, classProperty.PropertyInfo.PropertyType);
                    var body = (Expression)Expression.Call(entity, classProperty.PropertyInfo.SetMethod!, converted);

                    return Expression
                        .Lambda<Action<TEntity, object?>>(body, entity, value)
                        .Compile();
                }
                else
                    return null;
            });
        }
    }

    #endregion
}
