using System.Data.Common;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb.Reflection;

partial class Compiler
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="inputFields"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="dbSetting"></param>
    /// <param name="dbHelper"></param>
    /// <returns></returns>
    internal static Action<DbCommand, IList<object?>> CompileDictionaryStringObjectListDbParameterSetter(Type entityType,
        IEnumerable<DbField>? inputFields,
        int batchSize,
        IDbSetting dbSetting,
        IDbHelper? dbHelper)
    {
        var typeOfListEntity = typeof(IList<>).MakeGenericType(StaticType.Object);
        var getItemMethod = typeOfListEntity.GetMethod("get_Item", new[] { StaticType.Int32 })!;
        var dbCommandExpression = Expression.Parameter(StaticType.DbCommand, "command");
        var entitiesParameterExpression = Expression.Parameter(typeOfListEntity, "entities");
        var dbParameterCollectionExpression = Expression.Property(dbCommandExpression,
            StaticType.DbCommand.GetProperty(nameof(DbCommand.Parameters))!);
        var bodyExpressions = new List<Expression>
        {
            // Clear the parameter collection first
            GetDbParameterCollectionClearMethodExpression(dbParameterCollectionExpression)
        };

        // Iterate by batch size
        for (var entityIndex = 0; entityIndex < batchSize; entityIndex++)
        {
            var currentInstanceExpression = Expression.Call(entitiesParameterExpression, getItemMethod, Expression.Constant(entityIndex));
            var dictionaryInstanceExpression = ConvertExpressionToTypeExpression(currentInstanceExpression, StaticType.IDictionaryStringObject);

            // Iterate the fields
            foreach (var dbField in inputFields)
            {
                var dictionaryParameterExpression = GetDictionaryStringObjectParameterAssignmentExpression(dbCommandExpression,
                    entityIndex,
                    dictionaryInstanceExpression,
                    dbField,
                    dbSetting,
                    dbHelper);

                // Add to body
                bodyExpressions.Add(dictionaryParameterExpression);
            }
        }

        // Compile
        return Expression
            .Lambda<Action<DbCommand, IList<object?>>>(Expression.Block(bodyExpressions),
                dbCommandExpression,
                entitiesParameterExpression)
            .Compile();
    }
}
