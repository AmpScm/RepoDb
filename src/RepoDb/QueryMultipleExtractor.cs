using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;

namespace RepoDb;

/// <summary>
/// A class that is being used to extract the multiple resultsets of the 'ExecuteQueryMultiple' operation.
/// </summary>
public sealed class QueryMultipleExtractor : IDisposable, IAsyncDisposable
{
    /*
     * TODO: The extraction within this class does not use the DbFieldCache.Get() operation, therefore,
     *       we are not passing the values to the DataReader.ToEnumerable() method.
     */

    private readonly DbConnection? _connection;
    private readonly DbDataReader? _reader;
    private readonly bool _isDisposeConnection;
    private readonly object? _param;
    private readonly string? _cacheKey;
    private readonly int _cacheItemExpiration;
    private readonly ICache? _cache;
    private readonly List<object> _items = [];

    /// <summary>
    /// Creates a new instance of <see cref="QueryMultipleExtractor"/> class.
    /// </summary>
    /// <param name="connection">The instance of the <see cref="DbConnection"/> object that is current in used.</param>
    /// <param name="reader">The instance of the <see cref="DbDataReader"/> object to be extracted.</param>
    /// <param name="param">The parameter in used during the execution.</param>
    /// <param name="cacheKey">The key to the cached items.</param>
    /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
    /// <param name="cache">The cache object to be used.</param>
    /// <param name="isDisposeConnection">The flag that is used to define whether the associated <paramref name="connection"/> object will be disposed during the disposition process.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    internal QueryMultipleExtractor(DbConnection? connection = null,
        DbDataReader? reader = null,
        object? param = null,
        string? cacheKey = null,
        int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
        ICache? cache = null,
        bool isDisposeConnection = false,
        CancellationToken cancellationToken = default)
    {
        _connection = connection;
        _reader = reader;
        _isDisposeConnection = isDisposeConnection;
        _param = param;
        _cacheKey = cacheKey;
        _cacheItemExpiration = cacheItemExpiration;
        _cache = cache;
        Position = 0;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Disposes the current instance of <see cref="QueryMultipleExtractor"/>.
    /// </summary>
    public void Dispose()
    {
        // Reader
        _reader?.Dispose();

        // Connection
        if (_isDisposeConnection == true)
        {
            _connection?.Dispose();
        }

        // Set the output parameters
        DbConnectionExtension.SetOutputParameters(_param);
    }

#if !NET
    public ValueTask DisposeAsync()
    {
        Dispose();
        return new();
    }
#else
    public async ValueTask DisposeAsync()
    {
        // Reader
        if (_reader is { })
            await _reader.DisposeAsync().ConfigureAwait(false);

        // Connection
        if (_isDisposeConnection == true && _connection is { })
        {
            await _connection.DisposeAsync();
        }

        // Set the output parameters
        DbConnectionExtension.SetOutputParameters(_param);
    }
#endif

    #region Properties

    /// <summary>
    /// Gets the position of the <see cref="DbDataReader"/>.
    /// </summary>
    public int Position { get; private set; }

    /// <summary>
    /// Gets the instance of the <see cref="System.Threading.CancellationToken"/> currently in used.
    /// </summary>
    public CancellationToken CancellationToken { get; private set; }

    #endregion

    #region Cache

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool TryGetCacheItem<T>([NotNullWhen(true)] out T? value)
    {
        if (_cacheKey != null)
        {
            var cachedItem = _cache?.Get<object[]>(_cacheKey, false);

            if (cachedItem != null)
            {
                if (Position < cachedItem.Value.Length)
                {
                    value = (T)cachedItem.Value[Position];
                    return true;
                }
            }
        }

        value = default(T);

        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="item"></param>
    private void AddToCache(object item)
    {
        if (Position == 0)
        {
            _items.Clear();
        }

        _items.Add(item);

        if (_cacheKey != null)
        {
            var cachedItem = _cache?.Get<object[]>(_cacheKey, false);
            cachedItem?.Update(_items.AsArray(), _cacheItemExpiration, false);
        }
    }

    #endregion

    #region Extract

    #region Extract<TEntity>

    /// <summary>
    /// Extract the <see cref="DbDataReader"/> object into an enumerable of data entity objects.
    /// </summary>
    /// <typeparam name="TEntity">The type of data entity to be extracted.</typeparam>
    /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
    /// <returns>An enumerable of extracted data entity.</returns>
    public IEnumerable<TEntity> Extract<TEntity>(bool isMoveToNextResult = true)
    {
        if (TryGetCacheItem<IEnumerable<TEntity>>(out var result) == false)
        {
            result = DataReader.ToEnumerable<TEntity>(_reader, dbSetting: _connection?.GetDbSetting()).AsList();
            AddToCache(result);
        }

        if (isMoveToNextResult)
        {
            NextResult();
        }

        return result;
    }

    /// <summary>
    /// Extract the <see cref="DbDataReader"/> object into an enumerable of data entity objects in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of data entity to be extracted.</typeparam>
    /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
    /// <returns>An enumerable of extracted data entity.</returns>
    public async Task<IEnumerable<TEntity>> ExtractAsync<TEntity>(bool isMoveToNextResult = true, CancellationToken cancellationToken = default)
    {
        if (TryGetCacheItem<IEnumerable<TEntity>>(out var result) == false)
        {
            result = await DataReader
                .ToEnumerableAsync<TEntity>(_reader, dbSetting: _connection?.GetDbSetting(), cancellationToken: cancellationToken)
                .ToListAsync(CancellationToken).ConfigureAwait(false);
            AddToCache(result);
        }

        if (isMoveToNextResult)
        {
            await NextResultAsync(cancellationToken).ConfigureAwait(false);
        }

        return result;
    }

    public Task<IEnumerable<TEntity>> ExtractAsync<TEntity>(bool isMoveToNextResult)
        => this.ExtractAsync<TEntity>(isMoveToNextResult, this.CancellationToken);

    #endregion

    #region Extract<dynamic>

    /// <summary>
    /// Extract the <see cref="DbDataReader"/> object into an enumerable of dynamic objects.
    /// </summary>
    /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
    /// <returns>An enumerable of extracted data entity.</returns>
    public IEnumerable<dynamic> Extract(bool isMoveToNextResult = true)
    {
        if (TryGetCacheItem<IEnumerable<dynamic>>(out var result) == false)
        {
            result = DataReader.ToEnumerable(_reader).AsList();
            AddToCache(result);
        }

        if (isMoveToNextResult)
        {
            NextResult();
        }

        return result;
    }

    /// <summary>
    /// Extract the <see cref="DbDataReader"/> object into an enumerable of dynamic objects in an asynchronous way.
    /// </summary>
    /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
    /// <returns>An enumerable of extracted data entity.</returns>
    public async Task<IEnumerable<dynamic>> ExtractAsync(bool isMoveToNextResult = true)
    {
        if (TryGetCacheItem<IEnumerable<dynamic>>(out var result) == false)
        {
            result = await DataReader.ToEnumerableAsync(_reader, cancellationToken: CancellationToken)
                .ToListAsync(CancellationToken).ConfigureAwait(false);
            AddToCache(result);
        }

        if (isMoveToNextResult)
        {
            await NextResultAsync().ConfigureAwait(false);
        }
        return result;
    }

    #endregion

    #endregion

    #region Scalar

    #region Scalar<TResult>

    /// <summary>
    /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
    /// <returns>An instance of extracted object as value result.</returns>
    public TResult? Scalar<TResult>(bool isMoveToNextResult = true)
    {
        if (TryGetCacheItem<TResult>(out var result) == false)
        {
            if (_reader?.Read() == true)
            {
                result = Converter.ToType<TResult>(_reader[0]);
                AddToCache(result);
            }
        }

        if (isMoveToNextResult)
        {
            NextResult();
        }

        return result;
    }

    /// <summary>
    /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object in an asynchronous way.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
    /// <returns>An instance of extracted object as value result.</returns>
    public async Task<TResult?> ScalarAsync<TResult>(bool isMoveToNextResult = true)
    {
        if (TryGetCacheItem<TResult>(out var result) == false)
        {
            if (await _reader.ReadAsync(CancellationToken).ConfigureAwait(false))
            {
                result = Converter.ToType<TResult>(_reader[0]);
                AddToCache(result);
            }
        }

        if (isMoveToNextResult)
        {
            await NextResultAsync().ConfigureAwait(false);
        }

        return result;
    }

    #endregion

    #region Scalar<object>

    /// <summary>
    /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object.
    /// </summary>
    /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
    /// <returns>An instance of extracted object as value result.</returns>
    public object? Scalar(bool isMoveToNextResult = true) =>
        Scalar<object>(isMoveToNextResult);

    /// <summary>
    /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object in an asynchronous way.
    /// </summary>
    /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
    /// <returns>An instance of extracted object as value result.</returns>
    public Task<object?> ScalarAsync(bool isMoveToNextResult = true) =>
        ScalarAsync<object?>(isMoveToNextResult);

    #endregion

    #endregion

    #region NextResult

    /// <summary>
    /// Advances the <see cref="DbDataReader"/> object to the next result.
    /// <returns>True if there are more result sets; otherwise false.</returns>
    /// </summary>
    public bool NextResult() =>
        _reader is { } && ((Position = _reader.NextResult() ? Position + 1 : -1) >= 0);

    /// <summary>
    /// Advances the <see cref="DbDataReader"/> object to the next result in an asynchronous way.
    /// <returns>True if there are more result sets; otherwise false.</returns>
    /// </summary>
    public async Task<bool> NextResultAsync() =>
        _reader is { } && ((Position = await _reader.NextResultAsync(CancellationToken).ConfigureAwait(false) ? Position + 1 : -1) >= 0);


    public async Task<bool> NextResultAsync(CancellationToken? cancellationToken) =>
        _reader is { } && ((Position = await _reader.NextResultAsync(cancellationToken ?? CancellationToken).ConfigureAwait(false) ? Position + 1 : -1) >= 0);

    #endregion
}
