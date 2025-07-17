#nullable enable
using RepoDb.Interfaces;

namespace RepoDb;

public static class TestExtensions
{
    public static string CreateInsert(
        this IStatementBuilder sb,
        string tableName,
        IEnumerable<Field> fields,
        DbField? primaryField = null,
        DbField? identityField = null,
        string? hints = null
        )
    {
        IEnumerable<DbField> keyFields = MakeKeyFields(primaryField, identityField);
        return sb.CreateInsert(tableName, fields, keyFields, hints);
    }

    public static string CreateInsertAll(
        this IStatementBuilder sb,
        string tableName,
        IEnumerable<Field> fields,
        int batchSize = 0,
        DbField? primaryField = null,
        DbField? identityField = null,
        string? hints = null)
    {
        IEnumerable<DbField> keyFields = MakeKeyFields(primaryField, identityField);

        return sb.CreateInsertAll(tableName, fields ?? [], batchSize, keyFields, hints);
    }

    public static string CreateMerge(
        this IStatementBuilder sb,
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field>? qualifiers = null,
        DbField? primaryField = null,
        DbField? identityField = null,
        string? hints = null)
    {
        IEnumerable<DbField> keyFields = MakeKeyFields(primaryField, identityField);
        return sb.CreateMerge(tableName, fields, null, keyFields, qualifiers ?? [], hints);
    }

    public static string CreateMergeAll(
        this IStatementBuilder sb,
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field> qualifiers,
        int batchSize = 0,
        DbField? primaryField = null,
        DbField? identityField = null,
        string? hints = null)
    {
        IEnumerable<DbField> keyFields = MakeKeyFields(primaryField, identityField);
        return sb.CreateMergeAll(tableName, fields, null, qualifiers, batchSize, keyFields, hints);
    }

    public static string CreateUpdate(
        this IStatementBuilder sb,
        string tableName,
        IEnumerable<Field> fields,
        QueryGroup? where,
        DbField? primaryField = null,
        DbField? identityField = null,
        string? hints = null)
    {
        IEnumerable<DbField> keyFields = MakeKeyFields(primaryField, identityField);
        return sb.CreateUpdate(tableName, fields, where, keyFields, hints);
    }

    public static string CreateUpdateAll(
        this IStatementBuilder sb,
        string tableName,
        IEnumerable<Field> fields,
        IEnumerable<Field> qualifiers,
        int batchSize,
        DbField? primaryField = null,
        DbField? identityField = null,
        string? hints = null)
    {
        IEnumerable<DbField> keyFields = MakeKeyFields(primaryField, identityField);
        return sb.CreateUpdateAll(tableName, fields, qualifiers, batchSize, keyFields, hints);
    }

    private static IEnumerable<DbField> MakeKeyFields(DbField? primaryField, DbField? identityField)
    {
        if (identityField?.IsIdentity == false)
            throw new InvalidOperationException(); // To make tests happy
        else if (primaryField?.IsPrimary == false)
            throw new InvalidOperationException(); // To make tests happy

        if (primaryField is null && identityField is null)
            return [];
        else if (primaryField is null && identityField is { })
            return [identityField];
        else if (primaryField is { } && (identityField is null || identityField == primaryField))
            return [primaryField];

        return [identityField!, primaryField!];
    }
}
