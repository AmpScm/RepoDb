using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using RepoDb.Exceptions;
using RepoDb.SqlServer.BulkOperations;

namespace RepoDb;

public static partial class SqlConnectionExtension
{
    #region WriteToServerInternal

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="entities"></param>
    /// <param name="mappings"></param>
    /// <param name="options"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="hasOrderingColumn"></param>
    /// <param name="transaction"></param>
    /// 
    /// <returns></returns>
    private static int WriteToServerInternal<TEntity>(SqlConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<BulkInsertMapItem>? mappings = null,
        SqlBulkCopyOptions options = default,
        int bulkCopyTimeout = 0,
        int batchSize = 0,
        bool hasOrderingColumn = false,
        SqlTransaction? transaction = null)
        where TEntity : class
    {
        // Throw an error if there are no mappings
        if (mappings?.Any() != true)
        {
            throw new MissingMappingException("There are no mapping(s) found for this operation.");
        }

        // Variables needed
        int result;

        // Actual Execution
        using (var sqlBulkCopy = new SqlBulkCopy(connection, options, transaction))
        {
            // Set the destinationtable
            Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

            // Set the timeout
            if (bulkCopyTimeout > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout);
            }

            // Set the batch size
            if (batchSize > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize);
            }

            // Add the order column
            if (hasOrderingColumn)
            {
                mappings = AddOrderColumnMapping(mappings);
            }

            // Add the mappings
            AddMappings(sqlBulkCopy, mappings);

            // Open the connection and do the operation
            connection.EnsureOpen();
            using (var reader = new DataEntityDataReader<TEntity>(tableName, entities, connection, transaction, hasOrderingColumn))
            {
                var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<SqlBulkCopy>("WriteToServer", [typeof(DbDataReader)]);
                writeToServerMethod?.Invoke(sqlBulkCopy, new[] { reader });
                result = reader.RecordsAffected;
            }

            // Ensure the result
            if (result <= 0)
            {
                // Set the return value
                var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
                    Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
                result = rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy) ?? 0;
            }
        }

        // Return the result
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="reader"></param>
    /// <param name="mappings"></param>
    /// <param name="options"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    private static int WriteToServerInternal(SqlConnection connection,
        string tableName,
        DbDataReader reader,
        IEnumerable<BulkInsertMapItem>? mappings = null,
        SqlBulkCopyOptions options = default,
        int bulkCopyTimeout = 0,
        int batchSize = 0,
        SqlTransaction? transaction = null)
    {
        // Throw an error if there are no mappings
        if (mappings?.Any() != true)
        {
            throw new MissingMappingException("There are no mapping(s) found for this operation.");
        }

        // Variables needed
        int result;

        // Actual Execution
        using (var sqlBulkCopy = new SqlBulkCopy(connection, options, transaction))
        {
            // Set the destinationtable
            Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

            // Set the timeout
            if (bulkCopyTimeout > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout);
            }

            // Set the batch size
            if (batchSize > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize);
            }

            // Add the mappings
            AddMappings(sqlBulkCopy, mappings);


            // Open the connection and do the operation
            connection.EnsureOpen();
            var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<SqlBulkCopy>("WriteToServer", [typeof(DbDataReader)]);
            writeToServerMethod?.Invoke(sqlBulkCopy, new[] { reader });

            // Set the return value
            var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
                Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
            result = rowsCopiedFieldOrProperty != null ? rowsCopiedFieldOrProperty(sqlBulkCopy) : reader.RecordsAffected;
        }

        // Return the result
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="dataTable"></param>
    /// <param name="rowState"></param>
    /// <param name="mappings"></param>
    /// <param name="options"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="hasOrderingColumn"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    private static int WriteToServerInternal(SqlConnection connection,
        string tableName,
        DataTable dataTable,
        DataRowState? rowState = null,
        IEnumerable<BulkInsertMapItem>? mappings = null,
        SqlBulkCopyOptions options = default,
        int bulkCopyTimeout = 0,
        int batchSize = 0,
        bool hasOrderingColumn = false,
        SqlTransaction? transaction = null)
    {
        // Throw an error if there are no mappings
        if (mappings?.Any() != true)
        {
            throw new MissingMappingException("There are no mapping(s) found for this operation.");
        }

        // Variables needed
        int result;

        // Actual Execution
        using (var sqlBulkCopy = new SqlBulkCopy(connection, options, transaction))
        {
            // Set the destinationtable
            Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

            // Set the timeout
            if (bulkCopyTimeout > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout);
            }

            // Set the batch size
            if (batchSize > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize);
            }

            // Add the order column
            if (hasOrderingColumn)
            {
                AddOrderColumn(dataTable);
                mappings = AddOrderColumnMapping(mappings);
            }

            // Add the mappings
            AddMappings(sqlBulkCopy, mappings);

            // Open the connection and do the operation
            connection.EnsureOpen();
            if (rowState.HasValue == true)
            {
                var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<SqlBulkCopy>("WriteToServer", [typeof(DataTable), typeof(DataRowState)]);
                writeToServerMethod?.Invoke(sqlBulkCopy, [dataTable, rowState.Value]);
            }
            else
            {
                var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<SqlBulkCopy>("WriteToServer", [typeof(DataTable)]);
                writeToServerMethod?.Invoke(sqlBulkCopy, new[] { dataTable });
            }

            // Set the result
            result = rowState == null ? dataTable.Rows.Count : GetDataRows(dataTable, rowState).Count();
        }

        // Return the result
        return result;
    }

    #endregion

    #region WriteToServerAsync

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="entities"></param>
    /// <param name="mappings"></param>
    /// <param name="options"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="hasOrderingColumn"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<int> WriteToServerInternalAsync<TEntity>(SqlConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<BulkInsertMapItem>? mappings = null,
        SqlBulkCopyOptions options = default,
        int bulkCopyTimeout = 0,
        int batchSize = 0,
        bool hasOrderingColumn = false,
        SqlTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        // Throw an error if there are no mappings
        if (mappings?.Any() != true)
        {
            throw new MissingMappingException("There are no mapping(s) found for this operation.");
        }

        // Variables needed
        int result;

        // Actual Execution
        using (var sqlBulkCopy = new SqlBulkCopy(connection, options, transaction))
        {
            // Set the destinationtable
            Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

            // Set the timeout
            if (bulkCopyTimeout > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout);
            }

            // Set the batch size
            if (batchSize > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize);
            }

            // Add the order column
            if (hasOrderingColumn)
            {
                mappings = AddOrderColumnMapping(mappings);
            }

            // Add the mappings
            AddMappings(sqlBulkCopy, mappings);

            // Open the connection and do the operation
            await connection.EnsureOpenAsync(cancellationToken: cancellationToken);
            using (var reader = new DataEntityDataReader<TEntity>(tableName, entities, connection, transaction, hasOrderingColumn))
            {
                var writeToServerMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopy, Task>("WriteToServerAsync", [typeof(DbDataReader), typeof(CancellationToken)]);
                if (writeToServerMethod is { })
                    await writeToServerMethod(sqlBulkCopy, [reader, cancellationToken]);
                result = reader.RecordsAffected;
            }

            // Ensure the result
            if (result <= 0)
            {
                // Set the return value
                var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
                    Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
                result = rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy) ?? 0;
            }
        }

        // Return the result
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="reader"></param>
    /// <param name="mappings"></param>
    /// <param name="options"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<int> WriteToServerInternalAsync(SqlConnection connection,
        string tableName,
        DbDataReader reader,
        IEnumerable<BulkInsertMapItem>? mappings = null,
        SqlBulkCopyOptions options = default,
        int bulkCopyTimeout = 0,
        int batchSize = 0,
        SqlTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        // Throw an error if there are no mappings
        if (mappings?.Any() != true)
        {
            throw new MissingMappingException("There are no mapping(s) found for this operation.");
        }

        // Variables needed
        int result;

        // Actual Execution
        using (var sqlBulkCopy = new SqlBulkCopy(connection, options, transaction))
        {
            // Set the destinationtable
            Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

            // Set the timeout
            if (bulkCopyTimeout > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout);
            }

            // Set the batch size
            if (batchSize > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize);
            }

            // Add the mappings
            AddMappings(sqlBulkCopy, mappings);

            // Open the connection and do the operation
            await connection.EnsureOpenAsync(cancellationToken);
            var writeToServerMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopy, Task>("WriteToServerAsync", [typeof(DbDataReader), typeof(CancellationToken)]);
            if (writeToServerMethod is { })
                await writeToServerMethod(sqlBulkCopy, [reader, cancellationToken]);

            // Set the return value
            var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
                Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
            result = rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy) ?? reader.RecordsAffected;
        }

        // Return the result
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="tableName"></param>
    /// <param name="dataTable"></param>
    /// <param name="rowState"></param>
    /// <param name="mappings"></param>
    /// <param name="options"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="hasOrderingColumn"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<int> WriteToServerInternalAsync(SqlConnection connection,
        string tableName,
        DataTable dataTable,
        DataRowState? rowState = null,
        IEnumerable<BulkInsertMapItem>? mappings = null,
        SqlBulkCopyOptions options = default,
        int bulkCopyTimeout = 0,
        int batchSize = 0,
        bool hasOrderingColumn = false,
        SqlTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        // Throw an error if there are no mappings
        if (mappings?.Any() != true)
        {
            throw new MissingMappingException("There are no mapping(s) found for this operation.");
        }

        // Variables needed
        int result;

        // Actual Execution
        using (var sqlBulkCopy = new SqlBulkCopy(connection, options, transaction))
        {
            // Set the destinationtable
            Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

            // Set the timeout
            if (bulkCopyTimeout > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout);
            }

            // Set the batch size
            if (batchSize > 0)
            {
                Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize);
            }

            // Add the order column
            if (hasOrderingColumn)
            {
                AddOrderColumn(dataTable);
                mappings = AddOrderColumnMapping(mappings);
            }

            // Add the mappings
            AddMappings(sqlBulkCopy, mappings);

            // Open the connection and do the operation
            await connection.EnsureOpenAsync(cancellationToken);
            if (rowState.HasValue == true)
            {
                var writeToServerMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopy, Task>("WriteToServerAsync", [typeof(DataTable), typeof(DataRowState), typeof(CancellationToken)]);
                if (writeToServerMethod is { })
                    await writeToServerMethod(sqlBulkCopy, [dataTable, rowState.Value, cancellationToken]);
            }
            else
            {
                var writeToServerMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopy, Task>("WriteToServerAsync", [typeof(DataTable), typeof(CancellationToken)]);
                if (writeToServerMethod is { })
                    await writeToServerMethod(sqlBulkCopy, [dataTable, cancellationToken]);
            }

            // Set the result
            result = rowState == null ? dataTable.Rows.Count : GetDataRows(dataTable, rowState).Count();
        }

        // Return the result
        return result;
    }

    #endregion
}
