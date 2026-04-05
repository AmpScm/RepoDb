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

    internal static Func<DbDataReader, TResult> GetDataReaderToTypeCompiledFunction<TResult>(DbDataReader reader,
        DbFieldCollection? dbFields = null) =>
        DataReaderToTypeCache<TResult>.Get(reader, dbFields);

    #region DataReaderToTypeCache

    private static class DataReaderToTypeCache<TResult>
    {
        private static readonly ConcurrentDictionary<int, Func<DbDataReader, TResult>> cache = new();

        internal static Func<DbDataReader, TResult> Get(DbDataReader reader,
            DbFieldCollection? dbFields = null)
        {
            var key = HashCode.Combine(GetReaderFieldsHashCode(reader), typeof(TResult), dbFields);
            return cache.GetOrAdd(key, (_) => Compiler.CompileDataReaderToType<TResult>(reader, dbFields));
        }
    }

    #endregion

    #endregion

    #region GetDataReaderToExpandoObjectCompileFunction

    internal static Func<DbDataReader, dynamic> GetDataReaderToExpandoObjectCompileFunction(DbDataReader reader,
        DbFieldCollection? dbFields = null) =>
        DataReaderToExpandoObjectCache.Get(reader, dbFields);

    #region DataReaderToExpandoObjectCache

    private static class DataReaderToExpandoObjectCache
    {
        private static readonly ConcurrentDictionary<int, Func<DbDataReader, dynamic>> cache = new();

        internal static Func<DbDataReader, dynamic> Get(DbDataReader reader,
            DbFieldCollection? dbFields = null)
        {
            var key = HashCode.Combine(GetReaderFieldsHashCode(reader), dbFields);

            return cache.GetOrAdd(key, (_) => Compiler.CompileDataReaderToExpandoObject(reader, dbFields));
        }
    }

    #endregion

    #endregion

    #region GetDataEntityDbParameterSetterCompiledFunction

    internal static Action<DbCommand, object?> GetDataEntityDbParameterSetterCompiledFunction(Type entityType,
        string cacheKey,
        IEnumerable<DbField> inputFields,
        IEnumerable<DbField>? outputFields,
        IDbSetting dbSetting,
        IDbHelper dbHelper) =>
        DataEntityDbParameterSetterCache.Get(entityType,
            cacheKey,
            inputFields,
            outputFields,
            dbSetting,
            dbHelper);

    #region DataEntityDbParameterSetterCache

    private static class DataEntityDbParameterSetterCache
    {
        private static readonly ConcurrentDictionary<(Type Type, string CacheKey, int Key), Action<DbCommand, object?>> _cache = new();

        internal static Action<DbCommand, object?> Get(Type entityType,
            string cacheKey,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField>? outputFields,
            IDbSetting dbSetting,
            IDbHelper dbHelper)
        {
            var key = GetKey(entityType, cacheKey, inputFields, outputFields);

            return _cache.GetOrAdd(key, (_) =>
                TypeCache.Get(entityType).IsDictionaryStringObject
                ? Compiler.CompileDictionaryStringObjectDbParameterSetter(inputFields, dbSetting, dbHelper)
                : Compiler.CompileDataEntityDbParameterSetter(entityType, inputFields, outputFields, dbSetting, dbHelper)
                );
        }

        private static (Type, string, int) GetKey(Type entityType,
            string cacheKey,
            IEnumerable<DbField>? inputFields,
            IEnumerable<DbField>? outputFields)
        {
            int key = 722;
            if (inputFields is not null)
            {
                foreach (var field in inputFields)
                {
                    key = HashCode.Combine(key, field);
                }
            }
            if (outputFields is not null)
            {
                foreach (var field in outputFields)
                {
                    key = HashCode.Combine(key, field);
                }
            }
            return (entityType, cacheKey, key);
        }
    }

    #endregion

    #endregion

    #region GetDataEntityListDbParameterSetterCompiledFunction

    internal static Action<DbCommand, IList<object?>> GetDataEntityListDbParameterSetterCompiledFunction(Type entityType,
        string cacheKey,
        IEnumerable<DbField> inputFields,
        IEnumerable<DbField>? outputFields,
        int batchSize,
        IDbSetting dbSetting,
        IDbHelper? dbHelper = null) =>
        DataEntityListDbParameterSetterCache.Get(entityType, cacheKey, inputFields, outputFields, batchSize, dbSetting, dbHelper);

    #region DataEntityListDbParameterSetterCache

    private static class DataEntityListDbParameterSetterCache
    {
        private static readonly ConcurrentDictionary<(Type, string, int), Action<DbCommand, IList<object?>>> cache = new();

        internal static Action<DbCommand, IList<object?>> Get(Type entityType,
            string cacheKey,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField>? outputFields,
            int batchSize,
            IDbSetting dbSetting,
            IDbHelper? dbHelper = null)
        {
            var key = GetKey(entityType, cacheKey, inputFields, outputFields, batchSize);

            return cache.GetOrAdd(key, (_) =>
                TypeCache.Get(entityType).IsDictionaryStringObject
                ? Compiler.CompileDictionaryStringObjectListDbParameterSetter(
                        inputFields,
                        batchSize,
                        dbSetting,
                        dbHelper)
                : Compiler.CompileDataEntityListDbParameterSetter(
                        entityType,
                        inputFields,
                        outputFields,
                        batchSize,
                        dbSetting,
                        dbHelper));
        }

        private static (Type, string, int) GetKey(Type entityType,
            string cacheKey,
            IEnumerable<DbField>? inputFields,
            IEnumerable<DbField>? outputFields,
            int batchSize)
        {
            int key = batchSize;

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
            return (entityType, cacheKey, key);
        }
    }

    #endregion

    #endregion

    #region GetDataEntityPropertySetterCompiledFunction

    internal static Action<object, object?> GetDataEntityPropertySetterCompiledFunction(Type entityType,
        Field field) =>
        DataEntityPropertySetterCache.Get(entityType, field);

    internal static Func<object, object?> GetDataEntityPropertyGetterCompiledFunction(Type entityType,
        Field field) =>
        DataEntityPropertyGetterCache.Get(entityType, field);

    #region DataEntityPropertySetterCache

    private static class DataEntityPropertySetterCache
    {
        private static readonly ConcurrentDictionary<(Type Type, Field Field), Action<object, object?>> cache = new();

        internal static Action<object, object?> Get(Type type,
            Field field)
        {
            var key = (type, field);
            return cache.GetOrAdd(key, (_) =>
                TypeCache.Get(type).IsDictionaryStringObject
                ? Compiler.CompileDictionaryStringObjectItemSetter(type, field)
                : Compiler.CompileDataEntityPropertySetter(type, field)
                );
        }
    }

    #endregion

    #region DataEntityPropertySetterCache

    private static class DataEntityPropertyGetterCache
    {
        private static readonly ConcurrentDictionary<(Type Type, Field Field), Func<object, object?>> cache = new();

        internal static Func<object, object?> Get(Type type,
            Field field)
        {
            var key = (type, field);
            return cache.GetOrAdd(key, (_) =>
                TypeCache.Get(type).IsDictionaryStringObject
                ? Compiler.CompileDictionaryStringObjectItemGetter(type, field)
                : Compiler.CompileDataEntityPropertyGetter(type, field)
                );
        }
    }

    #endregion

    #endregion

    #region GetPlainTypeToDbParametersCompiledFunction

    internal static Action<DbCommand, object>? GetPlainTypeToDbParametersCompiledFunction(Type paramType,
        Type? entityType,
        DbFieldCollection? dbFields = null) =>
        PlainTypeToDbParametersCompiledFunctionCache.Get(paramType, entityType, dbFields);

    #region PlainTypeToDbParametersCompiledFunctionCache

    private static class PlainTypeToDbParametersCompiledFunctionCache
    {
        private static readonly ConcurrentDictionary<(Type ParamType, Type? EntityType), Action<DbCommand, object>?> cache = new();

        internal static Action<DbCommand, object>? Get(Type paramType,
            Type? entityType,
            DbFieldCollection? dbFields = null)
        {
            ArgumentNullException.ThrowIfNull(paramType);

            var key = (paramType, entityType);
            return cache.GetOrAdd(key, (_) =>
                paramType.IsPlainType()
                ? Compiler.GetPlainTypeToDbParametersCompiledFunction(paramType, entityType, dbFields)
                : null
            );
        }
    }

    #endregion

    #endregion
}
