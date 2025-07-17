using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using RepoDb.Extensions;
using RepoDb.Resolvers;

namespace RepoDb.Reflection;

partial class Compiler
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="paramType"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <returns></returns>
    public static Action<DbCommand, object> GetPlainTypeToDbParametersCompiledFunction(Type paramType,
        Type? entityType,
        DbFieldCollection? dbFields = null)
    {
        var dbCommandExpression = Expression.Parameter(StaticType.DbCommand, "command");
        var entityParameterExpression = Expression.Parameter(StaticType.Object, "entity");
        var entityExpression = ConvertExpressionToTypeExpression(entityParameterExpression, paramType);
        var callExpressions = new List<Expression>();

        // Iterate
        foreach (var prop in PropertyCache.Get(paramType))
        {
            // Ensure it matches to atleast one param
            var entityProperty = PropertyCache.Get(entityType) is { } p
                ? (p.GetByPropertyName(prop.PropertyName) ?? p.GetByFieldName(prop.FieldName)) : null;

            // Variables
            var dbField = dbFields?.GetByFieldName(prop.FieldName);
            var targetProperty = (entityProperty ?? prop);
            var valueExpression = (Expression)Expression.Property(entityExpression, prop.PropertyInfo);

            // Add the value itself
            if (StaticType.IDbDataParameter.IsAssignableFrom(targetProperty.PropertyInfo.PropertyType))
            {
                // The 'valueExpression' is of type 'IDbDataParameter' itself

                #region DbParameter

                // Set the name
                var setNameExpression = GetDbParameterNameAssignmentExpression(valueExpression, prop.PropertyName);
                callExpressions.AddIfNotNull(setNameExpression);

                // DbCommand.Parameters.Add
                var addExpression = GetDbCommandParametersAddExpression(dbCommandExpression, valueExpression);
                callExpressions.Add(addExpression);

                #endregion
            }
            else
            {
                #region NewParameter

                var propertyType = targetProperty.PropertyInfo.PropertyType;
                var underlyingType = TypeCache.Get(propertyType).UnderlyingType;
                var valueType = GetPropertyHandlerSetMethodReturnType(prop, underlyingType) ?? underlyingType;
                var dbParameterExpression = Expression.Variable(StaticType.DbParameter, $"var{prop.PropertyName}");
                var parameterCallExpressions = new List<Expression>();

                // Create
                var createParameterExpression =
                    CreateDbParameterExpression(dbCommandExpression, prop.PropertyName, valueExpression);
                parameterCallExpressions.Add(
                    Expression.Assign(dbParameterExpression,
                        ConvertExpressionToTypeExpression(createParameterExpression, StaticType.DbParameter)));

                // Convert

                // DbType
                DbType? dbType;
                if (valueType.IsEnum)
                {
                    /*
                     * Note: The other data provider can coerce the Enum into its destination data type in the DB by default,
                     *       except for PostgreSQL. The code written below is only to address the issue for this specific provider.
                     */

                    if (!IsPostgreSqlUserDefined(dbField))
                    {
                        dbType = prop.DbType ??
                            valueType.GetDbType() ??
                            (dbField != null ? new ClientTypeToDbTypeResolver().Resolve(dbField.Type) : null) ??
                            (DbType?)GlobalConfiguration.Options.EnumDefaultDatabaseType;
                    }
                    else
                    {
                        dbType = default;
                    }
                }
                else if (dbField?.Type != null)
                {
                    valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, dbField.TypeNullable());
                    dbType = default;
                }
                else
                {
                    dbType = targetProperty.DbType ??
                        valueType.GetDbType() ??
                        new ClientTypeToDbTypeResolver().Resolve(valueType);
                }
                var setDbTypeExpression = GetDbParameterDbTypeAssignmentExpression(dbParameterExpression, dbType);
                parameterCallExpressions.AddIfNotNull(setDbTypeExpression);

                // PropertyHandler
                InvokePropertyHandlerViaExpression(
                    dbParameterExpression, prop, ref valueType, ref valueExpression);

                // Value
                var setValueExpression = GetDbParameterValueAssignmentExpression(dbParameterExpression,
                    valueExpression);
                parameterCallExpressions.AddIfNotNull(setValueExpression);

                // Size
                if (dbField?.Size > 0)
                {
                    var setSizeExpression = GetDbParameterSizeAssignmentExpression(dbParameterExpression, dbField.Size.Value);
                    parameterCallExpressions.AddIfNotNull(setSizeExpression);
                }

                // Table-Valued Parameters
                if (valueType == StaticType.DataTable)
                {
                    parameterCallExpressions.AddIfNotNull(EnsureTableValueParameterExpression(dbParameterExpression));
                }

                // Type map attributes
                var parameterPropertyValueSetterAttributesExpressions = GetParameterPropertyValueSetterAttributesAssignmentExpressions(
                    dbParameterExpression, targetProperty);
                parameterCallExpressions.AddRangeIfNotNullOrNotEmpty(parameterPropertyValueSetterAttributesExpressions);

                // DbCommand.Parameters.Add
                var addExpression = GetDbCommandParametersAddExpression(dbCommandExpression, dbParameterExpression);
                parameterCallExpressions.Add(addExpression);

                // Add the parameter block
                callExpressions.Add(Expression.Block([dbParameterExpression], parameterCallExpressions));

                #endregion
            }
        }

        // Return
        return Expression
            .Lambda<Action<DbCommand, object>>(Expression.Block(callExpressions), dbCommandExpression, entityParameterExpression)
            .Compile();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbCommandExpression"></param>
    /// <param name="parameterName"></param>
    /// <param name="valueExpression"></param>
    /// <returns></returns>
    public static Expression CreateDbParameterExpression(Expression dbCommandExpression,
        string parameterName,
        Expression valueExpression)
    {
        var methodInfo = GetDbCommandCreateParameterMethod();

        return Expression.Call(methodInfo,
        [
            dbCommandExpression,
            Expression.Constant(parameterName),
            ConvertExpressionToTypeExpression(valueExpression, StaticType.Object),
            Expression.Default(StaticType.DbTypeNullable),
        ]);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="paramterExpression"></param>
    /// <param name="classProperty"></param>
    /// <param name="valueType"></param>
    /// <param name="valueExpression"></param>
    public static void InvokePropertyHandlerViaExpression(Expression paramterExpression,
        ClassProperty classProperty,
        ref Type valueType,
        ref Expression valueExpression)
    {
        var (expression, type) = ConvertExpressionToPropertyHandlerSetExpressionTuple(valueExpression, paramterExpression, classProperty, valueType);
        if (type != null)
        {
            valueType = type;
            valueExpression = expression;
        }
    }
}
