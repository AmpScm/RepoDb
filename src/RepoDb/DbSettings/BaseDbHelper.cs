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
    /// Initializes a new instance of the <see cref="BaseDbHelper"/> class.
    /// </summary>
    /// <param name="dbResolver"></param>
    protected BaseDbHelper(IResolver<string, Type> dbResolver)
    {
        ArgumentNullException.ThrowIfNull(dbResolver);

        DbTypeResolver = dbResolver;
    }

    /// <inheritdoc/>
    public IResolver<string, Type> DbTypeResolver { get; protected init; }

    /// <inheritdoc/>
    public virtual DbParameter? CreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName)
    {
        return null;
    }

    /// <inheritdoc/>
    public ValueTask<DbParameter?> CreateTableParameterAsync(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName, CancellationToken cancellationToken = default)
    {
        return new(CreateTableParameter(connection, transaction, fieldType, values, parameterName));
    }

    /// <inheritdoc/>
    public virtual bool CanCreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values)
    {
        return CreateTableParameter(connection, transaction, fieldType, values, "Q") is not null;
    }


    /// <inheritdoc/>
    public virtual DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction? transaction)
    {
        return new()
        {
        };
    }

    /// <inheritdoc/>
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
        if (value is IFormattable f && value.GetType() is { } vt
            && vt.HandleAsStringForDB() && !TypeMapper.IsPassthrough(vt))
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

    /// <summary>
    /// Allows the db provider to tweak the <paramref name="command"/> before calling an operation that should support returning identity information
    /// </summary>
    /// <param name="command"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public virtual Func<object?>? PrepareForIdentityOutput(DbCommand command, string tableName) => null;

    /// <summary>
    /// Allows the db provider to tweak the <paramref name="command"/> before calling ExecuteNonQueryAsync for batch operations like UPDATE
    /// </summary>
    /// <param name="command"></param>
    /// <param name="count"></param>
    public virtual void PrepareForBatchOperation(DbCommand command, int count) { }

    /// <summary>
    /// Allows the db provider to tweak the <paramref name="command"/> before calling ExecuteReader or ExecuteReaderAsync for multi-returns
    /// </summary>
    /// <param name="command"></param>
    public virtual void PrepareForExecuteReader(DbCommand command) { }

    /// <summary>
    /// Gets the post-creation expression for a parameter. This is used to set the value of the parameter after it has been created. This is useful for parameters that require special handling,
    /// such as table-valued parameters or JSON parameters.
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
    /// Gets the JSON column type for the database connection. This is used to determine the column type to use for JSON columns. This is useful for databases that
    /// support JSON columns, such as MySQL and PostgreSQL in certain versions. By default, it returns null, which means that JSON columns are not supported.
    /// Derived classes can override this method to provide the appropriate JSON column type for their respective databases.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public virtual string? GetJsonColumnType(DbConnection sql, DbTransaction? transaction) => (sql.GetStatementBuilder() as BaseStatementBuilder)?.JsonColumnType;

    /// <summary>
    /// Gets the provider-specific type transforms. This is used to transform expressions from one type to another for specific database providers. This is useful for databases that have
    /// specific requirements for certain db specific types that are custom wrapped, such as Vectors. Check the sources of the common implementations to find the very
    /// few cases where this is used. The info is used by the expression compiler to create entities and db values
    /// </summary>
    protected static ConcurrentDictionary<(Type fromType, Type toType), Func<Expression, Expression?>> ProviderSpecificTypeTransforms => Compiler.ProviderSpecificTransforms;
}
