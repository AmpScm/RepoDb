using System.Collections;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.DbSettings;
public abstract class BaseDbHelper : IDbHelper
{
    protected BaseDbHelper(IResolver<string, Type> dbResolver)
    {
        ArgumentNullException.ThrowIfNull(dbResolver);

        DbTypeResolver = dbResolver;
    }

    public IResolver<string, Type> DbTypeResolver { get; protected init; }

    public virtual DbParameter? CreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName)
    {
        return null;
    }

    public ValueTask<DbParameter?> CreateTableParameterAsync(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values, string parameterName, CancellationToken cancellationToken = default)
    {
        return new(CreateTableParameter(connection, transaction, fieldType, values, parameterName));
    }

    public virtual string? CreateTableParameterText(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, string parameterName, IEnumerable values)
    {
        return null;
    }

    public virtual bool CanCreateTableParameter(IDbConnection connection, IDbTransaction? transaction, Type? fieldType, IEnumerable values)
    {
        return CreateTableParameter(connection, transaction, fieldType, values, "Q") is not null;
    }

    /// <inheritdoc />
    public virtual void DynamicHandler<TEventInstance>(TEventInstance instance, string key)
    { }

    public virtual DbRuntimeSetting GetDbConnectionRuntimeInformation(IDbConnection connection, IDbTransaction? transaction)
    {
        return new()
        {
        };
    }

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
    /// <returns>The converted value.</returns>
    public virtual object? ParameterValueToDb(object? value, IDbDataParameter parameter)
    {
        if (value is IFormattable f && value.GetType().HandleAsStringForDB())
        {
            return f.ToString(null, CultureInfo.InvariantCulture);
        }

        return value;
    }

    /// <inheritdoc />
    public virtual Func<object?>? PrepareForIdentityOutput(DbCommand command) => null;

    public virtual Expression? GetParameterPostCreationExpression(ParameterExpression dbParameterExpression, ParameterExpression? propertyExpression, DbField dbField)
    {
        var method = StaticType.IDbHelper.GetMethod(nameof(IDbHelper.DynamicHandler))!
            .MakeGenericMethod(dbParameterExpression.Type);
        return Expression.Call(Expression.Constant(this),
            method, dbParameterExpression, Expression.Constant("RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]"));
    }
}
