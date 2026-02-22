using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb.Reflection;

internal partial class Compiler
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="dbParameterExpression"></param>
    /// <param name="dbHelper"></param>
    /// <returns></returns>
    private static MethodCallExpression GetCompilerDbParameterPostCreationExpression(ParameterExpression dbParameterExpression,
        IDbHelper? dbHelper)
    {
        var method = StaticType.IDbHelper.GetMethod(nameof(IDbHelper.DynamicHandler))!
            .MakeGenericMethod(dbParameterExpression.Type);
        return Expression.Call(Expression.Constant(dbHelper),
            method, dbParameterExpression, Expression.Constant("RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]"));
    }
}
