using System.Data;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb;
public static partial class DbConnectionExtension
{
    #region UpsertInternalBase<TEntity>

    /// <summary>
    /// Upserts a data entity or dynamic object into an existing data in the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity or dynamic object to be merged.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    internal static TResult UpsertInternalBase<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null)
        where TEntity : class
    {
        // Variables needed
        var type = entity.GetType();
        var isDictionaryType = TypeCache.Get(type).IsDictionaryStringObject();
        var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);
        var primary = dbFields.GetPrimary();
        IEnumerable<ClassProperty>? properties = null;
        ClassProperty? primaryKey = null;

        qualifiers ??= dbFields.GetPrimaryFields()?.AsFields();

        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            throw new PrimaryFieldNotFoundException($"There are no qualifiers, nor primary keys for '{tableName}'.");
        }

        // Get the properties
        QueryGroup? where = null;
        if (isDictionaryType == false)
        {
            if (type.IsGenericType == true)
            {
                properties = type.GetClassProperties();
            }
            else
            {
                properties = PropertyCache.Get(type);
            }

            // Set the primary key
            primaryKey = properties.GetByMappedName(primary?.Name);

            where = CreateQueryGroupForUpsert(entity,
                properties,
                qualifiers);
        }
        else
        {
            where = CreateQueryGroupForUpsert((IDictionary<string, object>)entity,
                qualifiers);
        }

        // Validate
        if (where == null)
        {
            throw new Exceptions.InvalidExpressionException("The generated expression from the given qualifiers is not valid.");
        }

        // Execution variables
        var result = default(TResult);
        var exists = connection.Exists(tableName,
            where,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder);

        // Check the existence
        if (exists == true)
        {
            // Call the update operation
            var updateResult = connection.Update(tableName,
                entity,
                where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);

            // Check if there is result
            if (updateResult > 0)
            {
                if (isDictionaryType == false)
                {
                    if (primaryKey != null)
                    {
                        result = Converter.ToType<TResult>(primaryKey.PropertyInfo.GetValue(entity));
                    }
                }
                else
                {
                    var dictionary = (IDictionary<string, object>)entity;
                    if (primary != null && dictionary.TryGetValue(primary.Name, out var value))
                    {
                        result = Converter.ToType<TResult>(value);
                    }
                }
            }
        }
        else
        {
            // Call the insert operation
            var insertResult = connection.Insert(tableName,
                entity,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);

            // Set the result
            result = Converter.ToType<TResult>(insertResult)!;
        }

        // Return the result
        return result!;
    }

    #endregion

    #region UpsertAsyncInternalBase<TEntity>

    /// <summary>
    /// Upserts a data entity or dynamic object into an existing data in the database in an .
    /// </summary>
    /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table to be used.</param>
    /// <param name="entity">The data entity or dynamic object to be merged.</param>
    /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
    /// <param name="qualifiers">The list of qualifier fields to be used.</param>
    /// <param name="hints">The table hints to be used.</param>
    /// <param name="traceKey">The tracing key to be used.</param>
    /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
    /// <param name="transaction">The transaction to be used.</param>
    /// <param name="trace">The trace object to be used.</param>
    /// <param name="statementBuilder">The statement builder object to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    internal static async ValueTask<TResult> UpsertAsyncInternalBase<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field>? qualifiers = null,
        IEnumerable<Field>? fields = null,
        string? hints = null,
        int commandTimeout = 0,
        string? traceKey = TraceKeys.Merge,
        IDbTransaction? transaction = null,
        ITrace? trace = null,
        IStatementBuilder? statementBuilder = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Variables needed
        var type = entity.GetType() ?? typeof(TEntity);
        var isDictionaryType = TypeCache.Get(type).IsDictionaryStringObject();
        var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
        var primary = dbFields.GetPrimary();
        IEnumerable<ClassProperty>? properties = null;
        ClassProperty? primaryKey = null;

        // Check the qualifiers
        if (qualifiers?.Any() != true)
        {
            // Set the primary as the qualifier
            qualifiers = dbFields.GetPrimaryFields()?.AsFields();

            if (qualifiers is null)
            {
                throw new PrimaryFieldNotFoundException($"There is no primary found for '{tableName}'.");
            }
        }

        // Get the properties
        QueryGroup? where = null;
        if (isDictionaryType == false)
        {
            if (type.IsGenericType == true)
            {
                properties = type.GetClassProperties();
            }
            else
            {
                properties = PropertyCache.Get(type);
            }

            // Set the primary key
            primaryKey = properties.FirstOrDefault(p =>
                string.Equals(primary?.Name, p.GetMappedName(), StringComparison.OrdinalIgnoreCase));

            where = CreateQueryGroupForUpsert(entity,
                properties,
                qualifiers);
        }
        else
        {
            where = CreateQueryGroupForUpsert((IDictionary<string, object>)entity,
                qualifiers);
        }

        // Validate
        if (where == null)
        {
            throw new Exceptions.InvalidExpressionException("The generated expression from the given qualifiers is not valid.");
        }

        // Execution variables
        var result = default(TResult)!;
        var exists = await connection.ExistsAsync(tableName,
            where,
            hints: hints,
            commandTimeout: commandTimeout,
            traceKey: traceKey,
            transaction: transaction,
            trace: trace,
            statementBuilder: statementBuilder,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Check the existence
        if (exists == true)
        {
            // Call the update operation
            var updateResult = await connection.UpdateAsync(tableName,
                entity,
                where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            // Check if there is result
            if (updateResult > 0)
            {
                if (isDictionaryType == false)
                {
                    if (primaryKey != null)
                    {
                        result = Converter.ToType<TResult>(primaryKey.PropertyInfo.GetValue(entity))!;
                    }
                }
                else
                {
                    var dictionary = (IDictionary<string, object>)entity;
                    if (primary != null && dictionary.TryGetValue(primary.Name, out var value))
                    {
                        result = Converter.ToType<TResult>(value)!;
                    }
                }
            }
        }
        else
        {
            // Call the insert operation
            var insertResult = await connection.InsertAsync(tableName,
                entity,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            // Set the result
            result = Converter.ToType<TResult>(insertResult)!;
        }

        // Return the result
        return result;
    }

    #endregion
}
