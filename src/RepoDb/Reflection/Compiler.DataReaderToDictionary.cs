using System.Data.Common;
using System.Dynamic;
using System.Linq.Expressions;
using RepoDb.Extensions;

namespace RepoDb.Reflection;

internal partial class Compiler
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="dbFields"></param>
    /// 
    /// <returns></returns>
    public static Func<DbDataReader, ExpandoObject> CompileDataReaderToExpandoObject(DbDataReader reader,
        DbFieldCollection? dbFields)
    {
        var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
        var readerFields = GetDataReaderFields(reader, dbFields);
        var memberBindings = GetMemberBindingsForDictionary(readerParameterExpression,
            readerFields.AsList(), reader.GetType());

        // Throw an error if there are no matching at least one
        if (memberBindings.Count <= 0)
        {
            throw new InvalidOperationException($"There are no member bindings found from the ResultSet of the data reader.");
        }

        // Initialize the members
        var body = Expression.ListInit(Expression.New(StaticType.ExpandoObject), memberBindings);

        // Set the function value
        return Expression
            .Lambda<Func<DbDataReader, ExpandoObject>>(body, readerParameterExpression)
            .Compile();
    }
}
