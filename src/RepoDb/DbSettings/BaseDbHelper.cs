using System.Collections;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;

namespace RepoDb.DbSettings;
public abstract class BaseDbHelper : IDbHelper
{
    protected BaseDbHelper(IResolver<string, Type> dbResolver)
    {
        if (dbResolver is null)
            throw new ArgumentNullException(nameof(dbResolver));

        DbTypeResolver = dbResolver;
    }

    public IResolver<string, Type> DbTypeResolver { get; protected init; }

    public virtual DbParameter? CreateTableParameter(IDbConnection connection, IDbTransaction? transaction, DbType? dbType, IEnumerable values, string parameterName)
    {
        return null;
    }

    public ValueTask<DbParameter?> CreateTableParameterAsync(IDbConnection connection, IDbTransaction? transaction, DbType? dbType, IEnumerable values, string parameterName, CancellationToken cancellationToken = default)
    {
        return new(CreateTableParameter(connection, transaction, dbType, values, parameterName));
    }

    public virtual string? CreateTableParameterText(IDbConnection connection, IDbTransaction? transaction, string parameterName, IEnumerable values)
    {
        return null;
    }

    public virtual bool CanCreateTableParameter(IDbConnection connection, IDbTransaction? transaction, DbType? dbType, IEnumerable values)
    {
        return CreateTableParameter(connection, transaction, dbType, values, "Q") is not null;
    }

    /// <inheritdoc />
    public virtual void DynamicHandler<TEventInstance>(TEventInstance instance, string key)
    { }

    public virtual DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction transaction)
    {
        return new()
        {
        };
    }

    public virtual ValueTask<DbRuntimeSetting> GetDbConnectionRuntimeInformationAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        return new(GetDbConnectionRuntimeInformation(connection, transaction));
    }

    /// <inheritdoc />
    public abstract IEnumerable<DbField> GetFields(IDbConnection connection, string tableName, IDbTransaction? transaction = null);

    /// <inheritdoc />
    public virtual ValueTask<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection, string tableName, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) => new(GetFields(connection, tableName, transaction));

    /// <inheritdoc />
    public abstract IEnumerable<DbSchemaObject> GetSchemaObjects(IDbConnection connection, IDbTransaction? transaction = null);

    /// <inheritdoc />
    public virtual ValueTask<IEnumerable<DbSchemaObject>> GetSchemaObjectsAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) => new(GetSchemaObjects(connection, transaction));

    /// <inheritdoc />
    public virtual object? ParameterValueToDb(object? value) => value;

    /// <inheritdoc />
    public virtual Func<object?>? PrepareForIdentityOutput(DbCommand command) => null;
}
