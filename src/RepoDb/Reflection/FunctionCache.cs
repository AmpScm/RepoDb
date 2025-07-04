using System.Collections.Concurrent;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the compiled functions.
/// </summary>
internal static class FunctionCache
{
    #region Helpers

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static int GetReaderFieldsHashCode(DbDataReader reader)
    {
        int hashCode = reader?.FieldCount ?? 0;

        if (reader is not null)
        {
            for (var ordinal = 0; ordinal < reader.FieldCount; ordinal++)
            {
                hashCode = HashCode.Combine(hashCode, reader.GetName(ordinal), ordinal, reader.GetFieldType(ordinal));
            }
        }

        return hashCode;
    }

    #endregion

    #region GetDataReaderToDataEntityCompiledFunction

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="reader"></param>
    /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
    /// 
    /// <returns></returns>
    internal static Func<DbDataReader, TResult> GetDataReaderToTypeCompiledFunction<TResult>(DbDataReader reader,
        DbFieldCollection? dbFields = null) =>
        DataReaderToTypeCache<TResult>.Get(reader, dbFields);

    #region DataReaderToTypeCache

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    private static class DataReaderToTypeCache<TResult>
    {
        private static readonly ConcurrentDictionary<int, Func<DbDataReader, TResult>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// 
        /// <returns></returns>
        internal static Func<DbDataReader, TResult> Get(DbDataReader reader,
            DbFieldCollection? dbFields = null)
        {
            var key = GetKey(reader);
            return cache.GetOrAdd(key, valueFactory: l => FunctionFactory.CompileDataReaderToType<TResult>(reader, dbFields));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static int GetKey(DbDataReader reader) =>
            HashCode.Combine(GetReaderFieldsHashCode(reader), typeof(TResult));
    }

    #endregion

    #endregion

    #region GetDataReaderToExpandoObjectCompileFunction

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="dbFields"></param>
    /// 
    /// <returns></returns>
    internal static Func<DbDataReader, dynamic> GetDataReaderToExpandoObjectCompileFunction(DbDataReader reader,
        DbFieldCollection? dbFields = null) =>
        DataReaderToExpandoObjectCache.Get(reader, dbFields);

    #region DataReaderToExpandoObjectCache

    /// <summary>
    ///
    /// </summary>
    private static class DataReaderToExpandoObjectCache
    {
        private static readonly ConcurrentDictionary<int, Func<DbDataReader, dynamic>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// 
        /// <returns></returns>
        internal static Func<DbDataReader, dynamic> Get(DbDataReader reader,
            DbFieldCollection? dbFields = null)
        {
            var key = GetKey(reader);

            return cache.GetOrAdd(key, (_) => FunctionFactory.CompileDataReaderToExpandoObject(reader, dbFields));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static int GetKey(DbDataReader reader) =>
            GetReaderFieldsHashCode(reader);
    }

    #endregion

    #endregion

    #region GetDataEntityDbParameterSetterCompiledFunction

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="cacheKey"></param>
    /// <param name="inputFields"></param>
    /// <param name="outputFields"></param>
    /// <param name="dbSetting"></param>
    /// <param name="dbHelper"></param>
    /// <returns></returns>
    internal static Action<DbCommand, object?> GetDataEntityDbParameterSetterCompiledFunction(Type entityType,
        string cacheKey,
        IEnumerable<DbField>? inputFields,
        IEnumerable<DbField>? outputFields,
        IDbSetting? dbSetting = null,
        IDbHelper? dbHelper = null) =>
        DataEntityDbParameterSetterCache.Get(entityType,
            cacheKey,
            inputFields,
            outputFields,
            dbSetting,
            dbHelper);

    #region DataEntityDbParameterSetterCache

    /// <summary>
    ///
    /// </summary>
    private static class DataEntityDbParameterSetterCache
    {
        private static readonly ConcurrentDictionary<int, Action<DbCommand, object?>> _cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="inputFields"></param>
        /// <param name="outputFields"></param>
        /// <param name="dbSetting"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object?> Get(Type entityType,
            string cacheKey,
            IEnumerable<DbField>? inputFields,
            IEnumerable<DbField>? outputFields,
            IDbSetting? dbSetting,
            IDbHelper? dbHelper)
        {
            var key = GetKey(entityType, cacheKey, inputFields, outputFields);

            return _cache.GetOrAdd(key, (_) =>
                TypeCache.Get(entityType).IsDictionaryStringObject()
                ? FunctionFactory.CompileDictionaryStringObjectDbParameterSetter(entityType, inputFields, dbSetting, dbHelper)
                : FunctionFactory.CompileDataEntityDbParameterSetter(entityType, inputFields, outputFields, dbSetting, dbHelper)
                );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="inputFields"></param>
        /// <param name="outputFields"></param>
        /// <returns></returns>
        private static int GetKey(Type entityType,
            string cacheKey,
            IEnumerable<DbField>? inputFields,
            IEnumerable<DbField>? outputFields)
        {
            var key = HashCode.Combine(entityType, cacheKey);
            if (inputFields != null)
            {
                foreach (var field in inputFields)
                {
                    key = HashCode.Combine(key, field);
                }
            }
            if (outputFields != null)
            {
                foreach (var field in outputFields)
                {
                    key = HashCode.Combine(key, field);
                }
            }
            return key;
        }
    }

    #endregion

    #endregion

    #region GetDataEntityListDbParameterSetterCompiledFunction

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="cacheKey"></param>
    /// <param name="inputFields"></param>
    /// <param name="outputFields"></param>
    /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
    /// <param name="dbSetting"></param>
    /// <param name="dbHelper"></param>
    /// <returns></returns>
    internal static Action<DbCommand, IList<object?>> GetDataEntityListDbParameterSetterCompiledFunction(Type entityType,
        string cacheKey,
        IEnumerable<DbField>? inputFields,
        IEnumerable<DbField>? outputFields,
        int batchSize,
        IDbSetting? dbSetting = null,
        IDbHelper? dbHelper = null) =>
        DataEntityListDbParameterSetterCache.Get(entityType, cacheKey, inputFields, outputFields, batchSize, dbSetting, dbHelper);

    #region DataEntityListDbParameterSetterCache

    /// <summary>
    ///
    /// </summary>
    private static class DataEntityListDbParameterSetterCache
    {
        private static readonly ConcurrentDictionary<int, Action<DbCommand, IList<object?>>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="inputFields"></param>
        /// <param name="outputFields"></param>
        /// <param name="batchSize">The batch to use. Use 0 for auto-chunking.</param>
        /// <param name="dbSetting"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static Action<DbCommand, IList<object?>> Get(Type entityType,
            string cacheKey,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField>? outputFields,
            int batchSize,
            IDbSetting? dbSetting,
            IDbHelper? dbHelper = null)
        {
            var key = GetKey(entityType, cacheKey, inputFields, outputFields, batchSize);

            return cache.GetOrAdd(key, (_) =>
                TypeCache.Get(entityType).IsDictionaryStringObject()
                ? FunctionFactory.CompileDictionaryStringObjectListDbParameterSetter(entityType,
                        inputFields,
                        batchSize,
                        dbSetting,
                        dbHelper)
                : FunctionFactory.CompileDataEntityListDbParameterSetter(entityType,
                        inputFields,
                        outputFields,
                        batchSize,
                        dbSetting,
                        dbHelper));
        }

        private static int GetKey(Type entityType,
            string cacheKey,
            IEnumerable<DbField>? inputFields,
            IEnumerable<DbField>? outputFields,
            int batchSize)
        {
            var key = HashCode.Combine(entityType, batchSize, cacheKey);

            if (inputFields?.Any() == true)
            {
                foreach (var field in inputFields)
                {
                    key = HashCode.Combine(key, field);
                }
            }
            if (outputFields?.Any() == true)
            {
                foreach (var field in outputFields)
                {
                    key = HashCode.Combine(key, field);
                }
            }
            return key;
        }
    }

    #endregion

    #endregion

    #region GetDbCommandToPropertyCompiledFunction

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="field"></param>
    /// <param name="parameterName"></param>
    /// <param name="index"></param>
    /// <param name="dbSetting"></param>
    /// <returns></returns>
    internal static Action<TEntity, DbCommand> GetDbCommandToPropertyCompiledFunction<TEntity>(Field field,
        string parameterName,
        int index,
        IDbSetting? dbSetting = null)
        where TEntity : class =>
        DbCommandToPropertyCache<TEntity>.Get(field, parameterName, index, dbSetting);

    #region DbCommandToPropertyCache

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    private static class DbCommandToPropertyCache<TEntity>
        where TEntity : class
    {
        private static readonly ConcurrentDictionary<int, Action<TEntity, DbCommand>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parameterName"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Action<TEntity, DbCommand> Get(Field field,
            string parameterName,
            int index,
            IDbSetting? dbSetting = null)
        {
            var key = HashCode.Combine(typeof(TEntity), field, parameterName, index);
            return cache.GetOrAdd(key, (_) => FunctionFactory.CompileDbCommandToProperty<TEntity>(field, parameterName, index, dbSetting));
        }
    }

    #endregion

    #endregion

    #region GetDataEntityPropertySetterCompiledFunction

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    internal static Action<object, object?> GetDataEntityPropertySetterCompiledFunction(Type entityType,
        Field field) =>
        DataEntityPropertySetterCache.Get(entityType, field);

    #region DataEntityPropertySetterCache

    /// <summary>
    ///
    /// </summary>
    private static class DataEntityPropertySetterCache
    {
        private static readonly ConcurrentDictionary<int, Action<object, object?>> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static Action<object, object?> Get(Type type,
            Field field)
        {
            var key = HashCode.Combine(type, field);
            return cache.GetOrAdd(key, (_) =>
                TypeCache.Get(type).IsDictionaryStringObject()
                ? FunctionFactory.CompileDictionaryStringObjectItemSetter(type, field)
                : FunctionFactory.CompileDataEntityPropertySetter(type, field)
                );
        }
    }

    #endregion

    #endregion

    #region GetPlainTypeToDbParametersCompiledFunction

    /// <summary>
    ///
    /// </summary>
    /// <param name="paramType"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <returns></returns>
    internal static Action<DbCommand, object>? GetPlainTypeToDbParametersCompiledFunction(Type paramType,
        Type? entityType,
        DbFieldCollection? dbFields = null) =>
        PlainTypeToDbParametersCompiledFunctionCache.Get(paramType, entityType, dbFields);

    #region PlainTypeToDbParametersCompiledFunctionCache

    /// <summary>
    ///
    /// </summary>
    private static class PlainTypeToDbParametersCompiledFunctionCache
    {
        private static readonly ConcurrentDictionary<int, Action<DbCommand, object>?> cache = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="paramType"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object>? Get(Type paramType,
            Type? entityType,
            DbFieldCollection? dbFields = null)
        {
#if NET
            ArgumentNullException.ThrowIfNull(paramType);
#else
            if (paramType == null)
                throw new ArgumentNullException(nameof(paramType));
#endif

            var key = HashCode.Combine(paramType, entityType);
            return cache.GetOrAdd(key, (_) =>
                paramType.IsPlainType()
                ? FunctionFactory.GetPlainTypeToDbParametersCompiledFunction(paramType, entityType, dbFields)
                : null
            );
        }
    }

    #endregion

    #endregion
}
