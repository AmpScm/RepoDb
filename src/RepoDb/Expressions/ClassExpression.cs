using System.Collections.Concurrent;
using System.Linq.Expressions;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb;

/// <summary>
/// A class used for manipulating class objects via expressions.
/// </summary>
public static partial class ClassExpression
{
    private static readonly ConcurrentDictionary<Type, object> getPropertyValuesCache = new();
    #region GetEntitiesPropertyValues

    /// <summary>
    /// Gets the values of the property of the data entities (via expression).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entities.</typeparam>
    /// <typeparam name="TResult">The result type of the extracted property.</typeparam>
    /// <param name="entities">The list of the data entities.</param>
    /// <param name="expression">The expression to be parsed.</param>
    /// <returns>The values of the property of the data entities.</returns>
    public static IEnumerable<TResult> GetEntitiesPropertyValues<TEntity, TResult>(IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object?>> expression)
        where TEntity : class
    {
        var property = ExpressionExtension.GetProperty(expression) ?? throw new PropertyNotFoundException(nameof(expression), "Property not found");
        var classProperty = PropertyCache.Get<TEntity>().GetByPropertyName(property.Name) ?? throw new PropertyNotFoundException(nameof(expression), "Property not found on class"); ;
        return GetEntitiesPropertyValues<TEntity, TResult>(entities, classProperty);
    }

    /// <summary>
    /// Gets the values of the property of the data entities (via property name).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entities.</typeparam>
    /// <typeparam name="TResult">The result type of the extracted property.</typeparam>
    /// <param name="entities">The list of the data entities.</param>
    /// <param name="propertyName">The name of the target property.</param>
    /// <returns>The values of the property of the data entities.</returns>
    public static IEnumerable<TResult> GetEntitiesPropertyValues<TEntity, TResult>(IEnumerable<TEntity> entities,
        string propertyName)
        where TEntity : class
    {
        var classProperty = PropertyCache.Get<TEntity>().GetByPropertyName(propertyName) ?? throw new PropertyNotFoundException(nameof(propertyName), "Property not found");
        return GetEntitiesPropertyValues<TEntity, TResult>(entities, classProperty);
    }

    /// <summary>
    /// Gets the values of the property of the data entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entities.</typeparam>
    /// <typeparam name="TResult">The result type of the extracted property.</typeparam>
    /// <param name="entities">The list of the data entities.</param>
    /// <param name="property">The target property.</param>
    /// <returns>The values of the property of the data entities.</returns>
    internal static IEnumerable<TResult> GetEntitiesPropertyValues<TEntity, TResult>(IEnumerable<TEntity> entities,
        ClassProperty property)
        where TEntity : class =>
        GetPropertyValuesCache<TEntity, TResult>.Do(entities, property);

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    private static class GetPropertyValuesCache<TEntity, TResult>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<int, Func<TEntity, TResult>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static Func<TEntity, TResult> GetFunc(ClassProperty property)
        {
            // Expressions
            var obj = Expression.Parameter(typeof(TEntity), "obj");

            // Set the body
            Expression body = Expression.Property(obj, property.PropertyInfo);

            // Convert if necessary
            if (property.PropertyInfo.PropertyType != typeof(TResult))
            {
                body = Expression.Convert(body, typeof(TResult));
            }

            // Set the function value
            return Expression
                .Lambda<Func<TEntity, TResult>>(body, obj)
                .Compile();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="property"></param>
        private static void Guard(ClassProperty property)
        {
#if NET
            ArgumentNullException.ThrowIfNull(property);
#else
            // Check the presence
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
#endif

            // Check the type (polymorphism)
            if (!property.DeclaringType.IsAssignableFrom(typeof(TEntity)))
            {
                throw new InvalidOperationException("The declaring type of the property is not equal to the target entity type.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Do(IEnumerable<TEntity> entities,
            ClassProperty property)
        {
            // Guard first
            Guard(property);

            // Variables needed
            var key = property.GetHashCode();

            // Get from the cache
            var func = cache.GetOrAdd(key, (_) => GetFunc(property));

            // Extract the values
            if (entities?.Any() == true)
            {
                foreach (var entity in entities)
                {
                    yield return func(entity);
                }
            }
        }
    }

    #endregion

    #region GetPropertiesAndValues

    /// <summary>
    /// Extract the class properties and values and returns an enumerable of <see cref="PropertyValue"/> object.
    /// </summary>
    /// <typeparam name="TEntity">The target type of the class.</typeparam>
    /// <param name="obj">The object to be extracted.</param>
    /// <returns>A list of <see cref="PropertyValue"/> object with extracted values.</returns>
    public static IEnumerable<PropertyValue> GetPropertiesAndValues<TEntity>(TEntity obj)
        where TEntity : class
    {
        var func = (Func<TEntity, IEnumerable<PropertyValue>>)getPropertyValuesCache.GetOrAdd(typeof(TEntity), (_) => GetFunc<TEntity>());

        return func(obj);
    }

    private static Func<TEntity, IEnumerable<PropertyValue>> GetFunc<TEntity>() where TEntity : class
    {
        // Expressions
        var obj = Expression.Parameter(typeof(TEntity), "obj");
        var addMethod = StaticType.PropertyValueList.GetMethod(nameof(List<PropertyValue>.Add), [StaticType.PropertyValue])!;
        var constructor = StaticType.PropertyValue.GetConstructor(
        [
                StaticType.String,
                StaticType.Object,
                StaticType.ClassProperty
            ])!;

        // Set the body
        var properties = PropertyCache.Get<TEntity>();
        var body = Expression.ListInit(
            Expression.New(StaticType.PropertyValueList),
            properties.Select(property =>
            {
                var name = Expression.Constant(property.FieldName);
                var value = Expression.Convert(Expression.Property(obj, property.PropertyInfo), StaticType.Object);
                var propertyValue = Expression.New(constructor,
                    name,
                    value,
                    Expression.Constant(property));
                return Expression.ElementInit(addMethod, propertyValue);
            }));

        // Set the function value
        return Expression
            .Lambda<Func<TEntity, IEnumerable<PropertyValue>>>(body, obj)
            .Compile();
    }


    #endregion
}
