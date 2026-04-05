using System.Data;
using System.Data.Common;
using System.Dynamic;
using NpgsqlTypes;
using RepoDb.Attributes.Parameter;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using RepoDb.Resolvers;

namespace RepoDb;

public static partial class NpgsqlConnectionExtension
{
    internal static IEnumerable<ClassProperty> GetMatchedProperties(DbFieldCollection dbFields,
        IEnumerable<ClassProperty> properties,
        bool includePrimary,
        bool includeIdentity,
        IDbSetting? dbSetting)
    {
        var matchedProperties = properties?
            .Where(property =>
            {
                var dbField = dbFields?.GetByFieldName(property.FieldName.AsUnquoted(true, dbSetting));

                return (dbField != null && dbField.IsPrimary == false && dbField.IsIdentity == false) ||
                       (includePrimary && dbField?.IsPrimary == true) ||
                       (includeIdentity && dbField?.IsIdentity == true);
            });

        if (matchedProperties?.Any() != true)
        {
            throw new InvalidOperationException($"There are no matching properties/columns found between the " +
                $"entity model and the underlying table.");
        }

        return matchedProperties;
    }

    private static NpgsqlBulkInsertMapItem? GetMapping(string sourceName,
        string destinationName,
        DbFieldCollection dbFields,
        bool includePrimary,
        bool includeIdentity,
        Type alternativeType,
        NpgsqlDbType? npgsqlDbType,
        IDbSetting? dbSetting)
    {
        if (npgsqlDbType is null)
        {
            var dbField = GetMappingDbField(destinationName,
                dbFields,
                includePrimary,
                includeIdentity,
                dbSetting);

            // Check
            if (dbField is null)
            {
                return default;
            }

            // Resolve
            npgsqlDbType = !string.IsNullOrWhiteSpace(dbField.DatabaseType) ?
                PostgreSqlDbTypeNameToNpgsqlDbTypeResolver.Instance.Resolve(dbField.DatabaseType ?? "TEXT") :
                ClientTypeToNpgsqlDbTypeResolver.Instance.Resolve(dbField.Type ?? alternativeType);
        }

        // Return
        return new NpgsqlBulkInsertMapItem(sourceName, destinationName, npgsqlDbType);
    }

    private static DbField? GetMappingDbField(string name,
        DbFieldCollection dbFields,
        bool includePrimary,
        bool includeIdentity,
        IDbSetting? dbSetting)
    {
        // Get
        var dbField = dbFields?.GetByFieldName(name.AsUnquoted(true, dbSetting));

        // Check
        if (dbField is null)
        {
            return dbField;
        }

        // Primary
        if ((dbField.IsPrimary && includePrimary) || // Primary
            (dbField.IsIdentity && includeIdentity) || // Identity
            (dbField.IsPrimary == false && dbField.IsIdentity == false)) // Others
        {
            return dbField;
        }

        // Return
        return null;
    }

    private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DbFieldCollection dbFields,
        IEnumerable<ClassProperty> properties,
        bool includePrimary,
        bool includeIdentity,
        IDbSetting? dbSetting)
    {
        var matchedProperties = GetMatchedProperties(dbFields,
            properties,
            includePrimary,
            includeIdentity,
            dbSetting);

        foreach (var property in matchedProperties)
        {
            var mapping = GetMapping(property.PropertyInfo.Name,
                property.FieldName,
                dbFields,
                includePrimary,
                includeIdentity,
                property.PropertyInfo.PropertyType,
                GetMappedNpgsqlDbTypeFromAttributes(property.GetPropertyValueAttributes()),
                dbSetting);

            if (mapping is not null)
            {
                yield return mapping;
            }
        }
    }

    private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IDictionary<string, object?> dictionary,
        DbFieldCollection dbFields,
        bool includePrimary,
        bool includeIdentity,
        IDbSetting? dbSetting)
    {
        foreach (var kvp in dictionary)
        {
            var mapping = GetMapping(kvp.Key,
                kvp.Key,
                dbFields,
                includePrimary,
                includeIdentity,
                TypeCache.Get(kvp.Value?.GetType()).UnderlyingType,
                null,
                dbSetting);

            if (mapping is not null)
            {
                yield return mapping;
            }
        }
    }

    private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DataTable table,
        DbFieldCollection dbFields,
        bool includePrimary,
        bool includeIdentity,
        IDbSetting? dbSetting)
    {
        foreach (DataColumn column in table.Columns)
        {
            var mapping = GetMapping(column.ColumnName,
                column.ColumnName,
                dbFields,
                includePrimary,
                includeIdentity,
                TypeCache.Get(column.DataType).UnderlyingType,
                null,
                dbSetting);

            if (mapping is not null)
            {
                yield return mapping;
            }
        }
    }

    private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DbDataReader reader,
        DbFieldCollection dbFields,
        bool includePrimary,
        bool includeIdentity,
        IDbSetting? dbSetting)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i);
            var mapping = GetMapping(name,
                name,
                dbFields,
                includePrimary,
                includeIdentity,
                TypeCache.Get(reader.GetFieldType(i)).UnderlyingType,
                null,
                dbSetting);

            if (mapping is not null)
            {
                yield return mapping;
            }
        }
    }

    private static NpgsqlDbType? GetMappedNpgsqlDbTypeFromAttributes(IEnumerable<PropertyValueAttribute> propertyValueAttributes)
    {
        // In purpose, the PropertyValueAttribute.Value is not exposed, therefore we cannot use this.
        // -> string.Equals(propertyValueAttribute.PropertyName, nameof(NpgsqlDbTypeAttribute.PropertyName), StringComparison.OrdinalIgnoreCase))

        var attribute = (NpgsqlDbTypeAttribute?)propertyValueAttributes
            .FirstOrDefault(propertyValueAttribute => propertyValueAttribute is NpgsqlDbTypeAttribute);

        return attribute?.NpgsqlDbType;
    }

    private static IEnumerable<DataRow> GetRows(DataTable table,
        DataRowState? rowState)
    {
        foreach (DataRow row in table.Rows)
        {
            if (rowState == null || row.RowState == rowState)
            {
                yield return row;
            }
        }
    }

    private static IEnumerable<ExpandoObject> GetExpandoObjectData<TData>(IEnumerable<TData> data,
        Field field) =>
        GetExpandoObjectData<TData>(data, field.FieldName);

    private static IEnumerable<ExpandoObject> GetExpandoObjectData<TData>(IEnumerable<TData> data,
        string column)
    {
        foreach (var item in data)
        {
            var expando = new ExpandoObject();
            var expandoObject = expando as IDictionary<string, object?>;
            expandoObject[column] = item;
            yield return expando;
        }
    }

    private static List<NpgsqlBulkInsertMapItem> AddOrderColumnMapping(IEnumerable<NpgsqlBulkInsertMapItem> mappings)
    {
        var list = new List<NpgsqlBulkInsertMapItem>(mappings);
        list.Insert(0,
            new NpgsqlBulkInsertMapItem("__RepoDb_OrderColumn", "__RepoDb_OrderColumn", NpgsqlDbType.Integer));
        return list;
    }

    private static void SetIdentities<TEntity>(Type entityType,
        IEnumerable<TEntity> entities,
        DbFieldCollection dbFields,
        IEnumerable<IdentityResult> identityResults,
        IDbSetting dbSetting)
        where TEntity : class
    {
        if (TypeCache.Get(entityType).IsDictionaryStringObject)
        {
            var dictionaries = entities.Select(item => (IDictionary<string, object?>)item);
            SetDictionaryIdentities(dictionaries, dbFields, identityResults, dbSetting);
        }
        else
        {
            SetEntityIdentities(entities, dbFields, identityResults, dbSetting);
        }
    }

    private static void SetEntityIdentities<TEntity>(IEnumerable<TEntity> entities,
        DbFieldCollection dbFields,
        IEnumerable<IdentityResult> identityResults,
        IDbSetting dbSetting)
        where TEntity : class =>
        SetEntityIdentities<TEntity>(entities, GetEntityIdentityField<TEntity>(dbFields, dbSetting), identityResults);

    private static void SetEntityIdentities<TEntity>(IEnumerable<TEntity> entities,
        Field? identityField,
        IEnumerable<IdentityResult> identityResults)
        where TEntity : class
    {
        if (identityField is null)
        {
            return;
        }

        var entityType = entities.FirstOrDefault()?.GetType() ?? typeof(TEntity);
        if (TypeCache.Get(entityType).IsClassType != true)
        {
            return;
        }

        var func = Compiler.GetPropertySetterFunc<TEntity>(identityField.FieldName);
        if (func is null)
        {
            return;
        }

        var entityList = entities.AsList();
        var bulkInsertIndex = -1;
        var index = 0;

        var targetPropertyType = PropertyCache.Get<TEntity>().First(p => p.PropertyInfo.Name == identityField.FieldName);
        var propType = targetPropertyType.PropertyInfo.PropertyType;

        foreach (var result in identityResults)
        {
            var entity = entityList[result.Index == bulkInsertIndex ? index : result.Index];
            object identityValue = result.Identity;

            //When using Return Identity operations the IdentityResult class returns the Identity as long
            //even though the Identity can be of type short or int so we need to convert it to the correct type
            if (identityValue is long longValue)
            {
                if (propType == typeof(short))
                {
                    identityValue = Convert.ToInt16(longValue);
                }
                else if (propType == typeof(int))
                {
                    identityValue = Convert.ToInt32(longValue);
                }
                else if (propType == typeof(short?))
                {
                    identityValue = (short?)Convert.ToInt16(longValue);
                }
                else if (propType == typeof(int?))
                {
                    identityValue = (int?)Convert.ToInt32(longValue);
                }
            }

            func(entity, identityValue);
            index++;
        }
    }

    private static void SetDictionaryIdentities(IEnumerable<IDictionary<string, object?>> entities,
        DbFieldCollection dbFields,
        IEnumerable<IdentityResult> identityResults,
        IDbSetting dbSetting)
    {
        var identityField = dbFields.Identity;

        if (identityField is null)
        {
            return;
        }

        var entityList = entities.AsList();
        var bulkInsertIndex = -1;
        var index = 0;

        foreach (var result in identityResults)
        {
            var entity = entityList[result.Index == bulkInsertIndex ? index : result.Index];
            entity[identityField.FieldName.AsUnquoted(true, dbSetting)] = result.Identity;
            index++;
        }
    }

    private static void SetDataTableIdentities(DataTable table,
        DbFieldCollection dbFields,
        IEnumerable<IdentityResult> identityResults,
        IDbSetting dbSetting)
    {
        var identityField = dbFields.Identity?.AsField();
        if (identityField is null)
        {
            return;
        }

        var identityColumn = GetDataTableIdentityColumn(table, identityField, dbSetting);
        identityColumn ??= table.Columns.Add(identityField.FieldName, identityField.Type ?? typeof(object));

        var bulkInsertIndex = -1;
        var index = 0;

        foreach (var result in identityResults)
        {
            var row = table.Rows[result.Index == bulkInsertIndex ? index : result.Index];
            row[identityColumn] = result.Identity;
            index++;
        }
    }

    private static Field? GetEntityIdentityField<TEntity>(DbFieldCollection dbFields,
        IDbSetting dbSetting)
        where TEntity : class
    {
        // We cannot use the ClassProperty.AsField() and PropertyInfo.AsField() as it is
        // using the mappings. We need to get the identity class property name instead

        var property = IdentityCache.Get<TEntity>() ??
                GetEntityIdentityProperty<TEntity>(dbFields, dbSetting);

        if (property is not null)
        {
            return new Field(property.PropertyInfo.Name);
        }

        return null;
    }

    private static ClassProperty? GetEntityIdentityProperty<TEntity>(DbFieldCollection dbFields,
        IDbSetting dbSetting)
        where TEntity : class
    {
        ClassProperty? property = null;

        if (dbFields?.Identity is { } identityDbField)
        {
            property = PropertyCache
               .Get<TEntity>()
               .FirstOrDefault(p =>
                   string.Equals(p.FieldName.AsUnquoted(true, dbSetting),
                       identityDbField.FieldName, StringComparison.OrdinalIgnoreCase));
        }

        return property;
    }

    private static DataColumn? GetDataTableIdentityColumn(DataTable table,
        Field identityField,
        IDbSetting dbSetting)
    {
        if (identityField is null)
        {
            return null;
        }

        foreach (DataColumn column in table.Columns)
        {
            if (string.Equals(column.ColumnName.AsUnquoted(true, dbSetting),
                identityField.FieldName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase))
            {
                return column;
            }
        }

        return null;
    }

    private static bool IsPrimaryAnIdentity(DbFieldCollection dbFields) =>
        IsPrimaryAnIdentity(dbFields.PrimaryFields?.OneOrDefault());

    private static bool IsPrimaryAnIdentity(DbField? primary) =>
        primary?.IsPrimary == true && primary?.IsIdentity == true;

    private static int EnumerableGetHashCode<T>(IEnumerable<T>? enumerable)
    {
        var hashCode = 0;

        if (enumerable?.Any() != true)
        {
            return hashCode;
        }

        foreach (var item in enumerable)
        {
            hashCode = HashCode.Combine(hashCode, item);
        }

        return hashCode;
    }
}
