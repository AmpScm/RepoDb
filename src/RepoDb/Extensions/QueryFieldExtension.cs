using System.Globalization;
using System.Text;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.Interfaces;

namespace RepoDb.Extensions;

/// <summary>
/// Contains the extension methods for <see cref="QueryField"/> object.
/// </summary>
public static class QueryFieldExtension
{
    /// <summary>
    /// Converts an instance of a query field into an enumerable list of query fields.
    /// </summary>
    /// <param name="queryField">The query field to be converted.</param>
    /// <returns>An enumerable list of query fields.</returns>
    public static IEnumerable<QueryField> AsEnumerable(this QueryField queryField)
    {
        yield return queryField;
    }

    /// <summary>
    /// Resets all the instances of <see cref="QueryField"/>.
    /// </summary>
    /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
    public static void ResetAll(this IEnumerable<QueryField> queryFields)
    {
        ArgumentNullException.ThrowIfNull(queryFields);
        foreach (var queryField in queryFields)
        {
            queryField.Reset();
        }
    }

    internal static void PrependAnUnderscoreAtParameter(this QueryField queryField, IDbSetting setting)
    {
        queryField.Parameter?.PrependAnUnderscoreAtParameter(setting);
    }

    internal static string AsField(this QueryField queryField,
        IDbSetting dbSetting) =>
        AsField(queryField, null, dbSetting);

    internal static string AsField(this QueryField queryField,
        string? functionFormat,
        IDbSetting? dbSetting) =>
        queryField.Field.FieldName.AsField(functionFormat, dbSetting);

    internal static string AsInParameter(this QueryField queryField,
        int index,
        IDbSetting? dbSetting)
    {
        var enumerable = ((System.Collections.IEnumerable)queryField.Parameter.Value!).AsTypedSet();

        if (!queryField.TableParameterMode)
        {
            int count = QueryField.RoundUpInLength(enumerable.Count);

            if (count >= dbSetting?.UseInValuesTreshold)
            {
                StringBuilder sb = new();

                sb.Append("(SELECT v FROM (VALUES ");

                for (int valueIndex = 0; valueIndex < count; valueIndex++)
                {
                    if (valueIndex > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append('(');
                    sb.Append($"{queryField.Parameter.Name}{(index > 0 ? index.ToString(CultureInfo.InvariantCulture) : "")}_In_{valueIndex.ToString(CultureInfo.InvariantCulture)}".AsParameter(dbSetting));
                    sb.Append(')');
                }
                sb.Append(") AS t(v) WHERE t.v IS NOT NULL)");
                return sb.ToString();
            }
            else
            {
                StringBuilder sb = new();

                sb.Append('(');

                for (int valueIndex = 0; valueIndex < count; valueIndex++)
                {
                    if (valueIndex > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append($"{queryField.Parameter.Name}{(index > 0 ? index.ToString(CultureInfo.InvariantCulture) : "")}_In_{valueIndex.ToString(CultureInfo.InvariantCulture)}".AsParameter(dbSetting));
                }
                sb.Append(')');
                return sb.ToString();
            }
        }
        else
        {
            return (dbSetting as BaseDbSetting)?.CreateTableParameterText(queryField.Parameter.Name.AsParameter(index, dbSetting, suffix: "_In_"))
                ?? $"(SELECT * FROM {queryField.Parameter.Name.AsParameter(index, dbSetting, suffix: "_In_")})";
        }
    }

    internal static string AsFieldAndParameterForBetween(
        this QueryField queryField,
        int index,
        string? functionFormat,
        IDbSetting? dbSetting)
    {
        return string.Concat(
            queryField.AsField(functionFormat, dbSetting), " ",
            queryField.Operation.GetText(),
            " ",
            queryField.Parameter.Name.AsParameter(index, dbSetting, suffix: "_Left"),
            " AND ",
            queryField.Parameter.Name.AsParameter(index, dbSetting, suffix: "_Right")
        );
    }

    internal static string AsFieldAndParameterForIn(this QueryField queryField,
        int index,
        string? functionFormat,
        IDbSetting? dbSetting)
    {
        var enumerable = (queryField.Parameter.Value as System.Collections.IEnumerable)?.WithType<object>();
        if (enumerable?.Any() != true)
        {
            return "1 = 0";
        }
        else
        {
            return string.Concat(queryField.AsField(functionFormat, dbSetting), " ", queryField.Operation.GetText(), " ", queryField.AsInParameter(index, /*, functionFormat*/ dbSetting));
        }
    }

    internal static QueryField? Negate(this QueryField from)
    {
        // Type is open for inheritance. This works for our implementations, but might not work for others
        Type type = from.GetType();
        if (type == typeof(QueryField))
            return new QueryField(from.Field, from.Operation.Negate(), from.Parameter, from.Parameter.DbType);
        else if (from is JsonExtractQueryField jsq)
            return new JsonExtractQueryField(jsq.FieldName, jsq.Path, jsq.Operation.Negate(), jsq.Parameter, jsq.Parameter.DbType);
        else if (from is FunctionalQueryField fqf && type.Assembly == typeof(FunctionalQueryField).Assembly)
            return new FunctionalQueryField(fqf.FieldName, from.Operation.Negate(), fqf.Parameter, fqf.Parameter.DbType, fqf.Format);

        return null;
    }
}
