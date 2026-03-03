using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Reflection;

internal partial class Compiler
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="field"></param>
    /// <param name="parameterName"></param>
    /// <param name="index"></param>
    /// <param name="dbSetting"></param>
    /// <returns></returns>
    public static Action<TEntity, DbCommand> CompileDbCommandToProperty<TEntity>(Field field,
        string parameterName,
        int index,
        IDbSetting? dbSetting)
        where TEntity : class
    {
        // Variables needed
        var typeOfEntity = typeof(TEntity);
        var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");
        var dbCommandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");

        // Variables for DbCommand
        var dbCommandParametersProperty = GetPropertyInfo<DbCommand>(x => x.Parameters);

        // Variables for DbParameterCollection
        var dbParameterCollectionIndexerMethod = StaticType.DbParameterCollection.GetMethod("get_Item", [StaticType.String])!;

        // Variables for DbParameter
        var dbParameterValueProperty = GetPropertyInfo<DbParameter>(x => x.Value);

        // Get the entity property
        var propertyName = field.FieldName.AsUnquoted(true, dbSetting).AsAlphaNumeric();
        var property = typeOfEntity.GetProperty(propertyName) ?? PropertyCache.Get(typeOfEntity).GetByFieldName(propertyName)?.PropertyInfo;
        if (property?.SetMethod is null)
            throw new PropertyNotFoundException(propertyName, $"Property {propertyName} not found");
        var propExpr = Expression.Property(Expression.Convert(entityParameterExpression, typeOfEntity), property);

        // Get the command parameter
        var name = parameterName ?? propertyName;
        var parameters = Expression.Property(dbCommandParameterExpression, dbCommandParametersProperty);
        var parameter = Expression.Call(parameters, dbParameterCollectionIndexerMethod,
            Expression.Constant(index > 0 ? string.Concat(name, "_", index.ToString(CultureInfo.InvariantCulture)) : name));

        // Assign the Parameter.Value into DataEntity.Property
        var value = Expression.Property(parameter, dbParameterValueProperty);
        var propertyAssignment = GetDbParameterValueAssignmentExpression(parameter, Expression.Convert(value, TypeCache.Get(field.Type).UnderlyingType));

        // Return function
        return Expression.Lambda<Action<TEntity, DbCommand>>(
            propertyAssignment, entityParameterExpression, dbCommandParameterExpression).Compile();
    }
}
