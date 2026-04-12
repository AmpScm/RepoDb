using System.Collections;
using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.StatementBuilders;

namespace RepoDb;

/// <summary>
/// A data reader object that is used to manipulate the enumerable list of data entity objects.
/// </summary>
/// <typeparam name="TEntity">The type of the data entity</typeparam>
#pragma warning disable CA1010 // Generic interface should also be implemented
public class DataEntityDataReader<TEntity> : DbDataReader
    where TEntity : class
{
    #region Fields
    private readonly int fieldCount; // 0
    private bool isClosed; // false
    private readonly bool isDictionaryStringObject; // false

    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
    /// </summary>
    /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
    public DataEntityDataReader(IEnumerable<TEntity> entities)
        : this(entities, null, null)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
    /// </summary>
    /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
    /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
    public DataEntityDataReader(IEnumerable<TEntity> entities,
        IDbConnection connection)
        : this(entities, connection, null)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
    /// </summary>
    /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
    /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    public DataEntityDataReader(IEnumerable<TEntity> entities,
        IDbConnection? connection,
        IDbTransaction? transaction)
        : this(null, entities, connection, transaction)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
    /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    public DataEntityDataReader(string? tableName,
        IEnumerable<TEntity> entities,
        IDbConnection? connection,
        IDbTransaction? transaction)
        : this(tableName, entities, connection, transaction, false)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
    /// </summary>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
    /// <param name="transaction">The transaction object that is currently in used.</param>
    /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
    /// <param name="hasOrderingColumn">The value that signifies whether the ordering column will be defined.</param>
    public DataEntityDataReader(string? tableName,
        IEnumerable<TEntity> entities,
        IDbConnection? connection = null,
        IDbTransaction? transaction = null,
        bool hasOrderingColumn = false)
    {
        ArgumentNullException.ThrowIfNull(entities);

        // Fields
        this.TableName = tableName ?? ClassMappedNameCache.Get<TEntity>();
        isClosed = false;
        IsDisposed = false;
        Position = -1;

        // Type
        var entityType = typeof(TEntity);
        EntityType = entityType == StaticType.Object ?
            (entities.FirstOrDefault()?.GetType() ?? entityType) :
            entityType;
        isDictionaryStringObject = TypeCache.Get(EntityType).IsDictionaryStringObject;

        // Properties
        Connection = connection;
        Transaction = transaction;
        HasOrderingColumn = hasOrderingColumn;
        EntityEnumerator = entities.GetEnumerator();
        Entities = entities;
        Properties = isDictionaryStringObject ? [] : PropertyCache.Get(EntityType).AsList();
        Fields = EnumerableExtension.AsList(GetFields(Entities.FirstOrDefault() as IDictionary<string, object?>));
        fieldCount = isDictionaryStringObject ? Fields.Count : Properties.Count;
    }

    /// <summary>
    /// The tablename of the entities being read if available
    /// </summary>
    public string TableName { get; }

    /// <inheritdoc/>
    public override IEnumerator GetEnumerator() =>
        Entities.GetEnumerator();

    /// <summary>
    /// Gets the instance of <see cref="IDbConnection"/> in used.
    /// </summary>
    public IDbConnection? Connection { get; }

    /// <summary>
    /// Gets the instance of <see cref="IDbTransaction"/> in used.
    /// </summary>
    public IDbTransaction? Transaction { get; }

    /// <summary>
    /// Gets the instance of enumerator that iterates through a collection of data entity objects.
    /// </summary>
    public IEnumerator<TEntity> EntityEnumerator { get; private set; }

    /// <summary>
    /// Gets the list of data entity objects.
    /// </summary>
    public IEnumerable<TEntity> Entities { get; private set; }

    /// <summary>
    /// Gets the current position of the enumerator.
    /// </summary>
    public int Position { get; private set; }

    /// <summary>
    /// Gets the type of the entities.
    /// </summary>
    private Type EntityType { get; }

    /// <summary>
    /// Gets the properties of data entity object.
    /// </summary>
    private List<ClassProperty> Properties { get; }

    /// <summary>
    /// Gets the fields of the dictionary.
    /// </summary>
    private List<Field> Fields { get; }

    /// <summary>
    /// Gets a value that indicates whether the ordering column is defined.
    /// </summary>
    private bool HasOrderingColumn { get; }

    /// <summary>
    /// Gets the current value from the index.
    /// </summary>
    /// <param name="i">The index of the column.</param>
    /// <returns>The value from the column index.</returns>
    public override object this[int i] => GetValue(i);

    /// <summary>
    /// Gets the current value from the name.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value from the column name.</returns>
    public override object this[string name] => this[GetOrdinal(name)];

    /// <summary>
    /// Gets the depth value.
    /// </summary>
    public override int Depth { get; }

    /// <summary>
    /// Gets the value that indicates whether the current reader is closed.
    /// </summary>
    public override bool IsClosed =>
        isClosed;

    /// <summary>
    /// Gets the value that indicates whether the current reader is already disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets the number of rows affected by the iteration (always returns 0).
    /// </summary>
    public override int RecordsAffected => 0;

    /// <summary>
    /// Gets the number of properties the data entity object has.
    /// </summary>
    public override int FieldCount => HasOrderingColumn ? fieldCount + 1 : fieldCount;

    /// <summary>
    /// Gets a value that signify whether the current data reader has data entities.
    /// </summary>
    public override bool HasRows =>
        Entities?.Any() == true;

    /// <summary>
    /// Closes the current data reader.
    /// </summary>
    public override void Close()
    {
        isClosed = true;
    }

    /// <summary>
    /// Disposes the current data reader.
    /// </summary>
    public new void Dispose()
    {
        base.Dispose();
        Entities = null!;
        EntityEnumerator = null!;
        Close();
        IsDisposed = true;
    }

    /// <summary>
    /// Resets the pointer of the position to the beginning.
    /// </summary>
    public void Reset()
    {
        ThrowExceptionIfNotAvailable();
        EntityEnumerator = Entities.GetEnumerator();
        Position = -1;
    }

    /// <summary>
    /// Gets the boolean value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override bool GetBoolean(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<bool>(ordinal);
    }

    /// <summary>
    /// Gets the byte value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override byte GetByte(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<byte>(ordinal);
    }

    /// <summary>
    /// GetBytes
    /// </summary>
    /// <param name="ordinal">Int</param>
    /// <param name="dataOffset">Int64</param>
    /// <param name="buffer">byte[]</param>
    /// <param name="bufferOffset">Int</param>
    /// <param name="length">Int</param>
    /// <returns></returns>
    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
    {
        ThrowExceptionIfNotAvailable();
        throw new NotSupportedException("This is not supported by this data reader.");
    }

    /// <summary>
    /// Gets the char value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override char GetChar(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<char>(ordinal);
    }

    /// <summary>
    /// GetChars
    /// </summary>
    /// <param name="ordinal">Int</param>
    /// <param name="dataOffset">Int64</param>
    /// <param name="buffer">char[]</param>
    /// <param name="bufferOffset">Int</param>
    /// <param name="length">Int</param>
    /// <returns></returns>
    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
    {
        ThrowExceptionIfNotAvailable();
        throw new NotSupportedException("This is not supported by this data reader.");
    }

    /// <summary>
    /// GetData
    /// </summary>
    /// <param name="i">Int</param>
    /// <returns>Int</returns>
    public new IDataReader GetData(int i)
    {
        ThrowExceptionIfNotAvailable();
        GC.KeepAlive(i);
        throw new NotSupportedException("This is not supported by this data reader.");
    }

    /// <summary>
    /// Gets the name of the property data type from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The property type name from the property index.</returns>
    public override string GetDataTypeName(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return Properties[ordinal].PropertyInfo.PropertyType.Name;
    }

    /// <summary>
    /// Gets the date time value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override DateTime GetDateTime(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<DateTime>(ordinal);
    }

    /// <summary>
    /// Gets the decimal value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override decimal GetDecimal(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<decimal>(ordinal);
    }

    /// <summary>
    /// Gets the double value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override double GetDouble(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<double>(ordinal);
    }

    /// <summary>
    /// Gets the type of the property from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The property type from the property index.</returns>
    public override Type GetFieldType(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return Properties[ordinal].PropertyInfo.PropertyType;
    }

    /// <summary>
    /// Gets the float value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override float GetFloat(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<float>(ordinal);
    }

    /// <summary>
    /// Gets the Guid value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override Guid GetGuid(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        var value = GetRawValue(ordinal);
        if (value is Guid g)
            return g;
        else if (value is string s)
            return Guid.Parse(s);

        return Guid.Parse(value?.ToString() ?? "");
    }

    /// <summary>
    /// Gets the short value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override short GetInt16(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<short>(ordinal);
    }

    /// <summary>
    /// Gets the int value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override int GetInt32(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<int>(ordinal);
    }

    /// <summary>
    /// Gets the long value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override long GetInt64(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<long>(ordinal);
    }

    /// <inheritdoc />
    public override T GetFieldValue<T>(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        var value = GetRawValue(ordinal);
        return value is T v ? v : Converter.ToType<T>(value ?? DBNull.Value);
    }

    /// <inheritdoc/>
    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
    {
        ThrowExceptionIfNotAvailable();
        var value = GetRawValue(ordinal);
        return Task.FromResult(value is null || Convert.IsDBNull(value));
    }

    /// <summary>
    /// Gets the name of the property from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The name from the property index.</returns>
    public override string GetName(int ordinal) =>
        isDictionaryStringObject ? GetNameForDictionaryStringObject(ordinal) : GetNameForEntities(ordinal);

    private string GetNameForEntities(int i)
    {
        ThrowExceptionIfNotAvailable();
        if (i == fieldCount && HasOrderingColumn)
        {
            return BaseStatementBuilder.RepoDbOrderColumn;
        }
        return Properties[i].FieldName;
    }

    private string GetNameForDictionaryStringObject(int i)
    {
        ThrowExceptionIfNotAvailable();
        if (i == fieldCount && HasOrderingColumn)
        {
            return BaseStatementBuilder.RepoDbOrderColumn;
        }
        return Fields[i].FieldName;
    }

    /// <summary>
    /// Gets the index of the property based on the property name.
    /// </summary>
    /// <param name="name">The index of the property.</param>
    /// <returns>The index of the property from property name.</returns>
    public override int GetOrdinal(string name) =>
        isDictionaryStringObject ? GetOrdinalForDictionaryStringObject(name) : GetOrdinalForEntities(name);

    private int GetOrdinalForEntities(string name)
    {
        ThrowExceptionIfNotAvailable();
        if (HasOrderingColumn && string.Equals(name, BaseStatementBuilder.RepoDbOrderColumn, StringComparison.OrdinalIgnoreCase))
        {
            return Properties.Count;
        }
        else
        {
            var property = Properties.GetByFieldName(name) ?? Properties.GetByPropertyName(name);
            return property is { } ? Properties.IndexOf(property) : -1;
        }
    }

    private int GetOrdinalForDictionaryStringObject(string name)
    {
        ThrowExceptionIfNotAvailable();
        if (HasOrderingColumn && string.Equals(name, BaseStatementBuilder.RepoDbOrderColumn, StringComparison.OrdinalIgnoreCase))
        {
            return Fields.Count;
        }
        else
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                if (string.Equals(Fields[i].FieldName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    /// <summary>
    /// Gets the table schema.
    /// </summary>
    /// <returns>An instance of the <see cref="DataTable"/> with the table schema.</returns>
    public override DataTable GetSchemaTable()
    {
        ThrowExceptionIfNotAvailable();
        throw new NotSupportedException("This is not supported by this data reader.");
    }

    /// <summary>
    /// Gets the string value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override string GetString(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        return GetFieldValue<string>(ordinal);
    }

    /// <summary>
    /// Gets the current value from the defined property index.
    /// </summary>
    /// <param name="ordinal">The index of the property.</param>
    /// <returns>The value from the property index.</returns>
    public override object GetValue(int ordinal) =>
        GetRawValue(ordinal) ?? DBNull.Value;

    private object? GetRawValue(int ordinal)
    {
        ThrowExceptionIfNotAvailable();

        if (ordinal == fieldCount && HasOrderingColumn)
        {
            return Position;
        }

        return isDictionaryStringObject
            ? ((EntityEnumerator.Current as IDictionary<string, object?>)?[Fields[ordinal].FieldName])
            : Properties[ordinal].PropertyInfo.GetValue(EntityEnumerator.Current);
    }

    /// <summary>
    /// Populates the values of the array of the current values of the current row.
    /// </summary>
    /// <param name="values">The array variable on which to populate the data.</param>
    /// <returns></returns>
    public override int GetValues(object?[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        ThrowExceptionIfNotAvailable();

        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, FieldCount, nameof(values));

        int i = 0;
        foreach (var v in ClassExpression.GetPropertiesAndValues(EntityEnumerator.Current))
        {
            values[i++] = v.Value;
        }
        if (HasOrderingColumn)
            values[i] = Position;
        return FieldCount;
    }

    /// <inheritdoc/>
    public override bool IsDBNull(int ordinal)
    {
        ThrowExceptionIfNotAvailable();
        var value = GetRawValue(ordinal);
        return value is null || Convert.IsDBNull(value);
    }

    /// <inheritdoc/>
    public override bool NextResult()
    {
        ThrowExceptionIfNotAvailable();
        return false;
    }

    /// <inheritdoc/>
    public override bool Read()
    {
        ThrowExceptionIfNotAvailable();
        Position++;
        return EntityEnumerator.MoveNext();
    }

    /// <inheritdoc/>
    public override Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        ThrowExceptionIfNotAvailable();
        Position++;
        return Task.FromResult(EntityEnumerator.MoveNext());
    }

    private void ThrowExceptionIfNotAvailable()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(DataEntityDataReader<>), "The reader is already disposed.");
        }
        if (IsClosed)
        {
            throw new InvalidOperationException("The reader is already closed.");
        }
    }

    private static IEnumerable<Field> GetFields(IDictionary<string, object?>? dictionary)
    {
        if (dictionary is not null)
        {
            foreach (var kvp in dictionary)
            {
                yield return new Field(kvp.Key, kvp.Value?.GetType());
            }
        }
    }
}
