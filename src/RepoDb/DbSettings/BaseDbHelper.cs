using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using RepoDb.StatementBuilders;

namespace RepoDb.DbSettings;

/// <summary>
/// Standard implementation of <see cref="IDbHelper"/>. Should be used as baseclass by every RepoDb type implementation.
/// </summary>
public abstract class BaseDbHelper : IDbHelper
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="dbResolver"></param>
    protected BaseDbHelper(IResolver<string, Type> dbResolver)
    {
        ArgumentNullException.ThrowIfNull(dbResolver);

        DbTypeResolver = dbResolver;
    }

    /// <summary>
    ///
    /// </summary>
    public IResolver<string, Type> DbTypeResolver { get; protected init; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="fieldType"></param>
    /// <param name="values"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    public virtual DbParameter? CreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName)
    {
        return null;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="fieldType"></param>
    /// <param name="values"></param>
    /// <param name="parameterName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<DbParameter?> CreateTableParameterAsync(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName, CancellationToken cancellationToken = default)
    {
        return new(CreateTableParameter(connection, transaction, fieldType, values, parameterName));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="fieldType"></param>
    /// <param name="parameterName"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual string? CreateTableParameterText(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, string parameterName, IEnumerable values)
    {
        return null;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="fieldType"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual bool CanCreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values)
    {
        return CreateTableParameter(connection, transaction, fieldType, values, "Q") is not null;
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public virtual DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction? transaction)
    {
        return new()
        {
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual ValueTask<DbRuntimeSetting> GetDbConnectionRuntimeInformationAsync(IDbConnection connection, IDbTransaction? transaction, CancellationToken cancellationToken)
    {
        return new(GetDbConnectionRuntimeInformation(connection, transaction));
    }

    /// <inheritdoc />
    public abstract DbFieldCollection GetFields(IDbConnection connection, string tableName, IDbTransaction? transaction = null);

    /// <inheritdoc />
    public virtual ValueTask<DbFieldCollection> GetFieldsAsync(IDbConnection connection, string tableName, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) => new(GetFields(connection, tableName, transaction));

    /// <inheritdoc />
    public abstract IEnumerable<DbSchemaObject> GetSchemaObjects(IDbConnection connection, IDbTransaction? transaction = null);

    /// <inheritdoc />
    public virtual ValueTask<IEnumerable<DbSchemaObject>> GetSchemaObjectsAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) => new(GetSchemaObjects(connection, transaction));

    /// <summary>
    /// Converts a raw value to a db valid valuetype. Used for setting <see cref="DbParameter.Value"/> on <see cref="DbParameter"/>
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="parameter"></param>
    /// <returns>The converted value.</returns>
    public virtual object? ParameterValueToDb(object? value, IDbDataParameter parameter)
    {
        if (value is IFormattable f && value.GetType().HandleAsStringForDB())
        {
            return f.ToString(null, CultureInfo.InvariantCulture);
        }
#if NET
        else if (value is Half h)
        {
            return (float)h;
        }
#endif

        return value;
    }

    /// <inheritdoc />
    public virtual Func<object?>? PrepareForIdentityOutput(DbCommand command) => null;

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="count"></param>
    public virtual void PrepareForBatchOperation(DbCommand command, int count) { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbParameterExpression"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="dbField"></param>
    /// <returns></returns>
    public virtual Expression? GetParameterPostCreationExpression(ParameterExpression dbParameterExpression, ParameterExpression? propertyExpression, DbField dbField)
    {
        return null;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public virtual string? GetJsonColumnType(DbConnection sql, DbTransaction transaction) => (sql.GetStatementBuilder() as BaseStatementBuilder)?.JsonColumnType;

    /// <summary>
    ///
    /// </summary>
    protected static ConcurrentDictionary<(Type fromType, Type toType), Func<Expression, Expression?>> ProviderSpecificTypeTransforms => Compiler.ProviderSpecificTransforms;
}
