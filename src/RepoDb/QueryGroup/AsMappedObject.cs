using System.Collections;
using System.Data;
using System.Dynamic;
using System.Globalization;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb;

public partial class QueryGroup
{
    /// <summary>
    /// Converts every <see cref="QueryGroup"/> object of the list of <see cref="QueryGroupTypeMap"/> into an <see cref="object"/>
    /// with all the child <see cref="QueryField"/>s as the property/value to that object. The value of every property of the created
    /// object will be an instance of the <see cref="CommandParameter"/> with the proper type, name and value.
    /// </summary>
    /// <param name="queryGroupTypeMaps">The list of <see cref="QueryGroupTypeMap"/> objects to be converted.</param>
    /// <param name="connection"></param>
    /// <returns>An instance of an object that contains all the definition of the converted underlying <see cref="QueryFields"/>s.</returns>
    /// <param name="transaction"></param><param name="fixParameters">A boolean value whether to fix the parameter name before converting.</param>
    internal static object AsMappedObject(IReadOnlyList<QueryGroupTypeMap> queryGroupTypeMaps,
        IDbConnection connection, IDbTransaction? transaction, string? tableName)
    {
        var dictionary = new ExpandoObject() as IDictionary<string, object?>;

        foreach (var queryGroupTypeMap in queryGroupTypeMaps)
        {
            AsMappedObject(dictionary, queryGroupTypeMap, tableName, connection, transaction);
        }

        return (ExpandoObject)dictionary;
    }

    /// <summary>
    /// Converts every <see cref="QueryGroup"/> object of the list of <see cref="QueryGroupTypeMap"/> into an <see cref="object"/>
    /// with all the child <see cref="QueryField"/>s as the property/value to that object. The value of every property of the created
    /// object will be an instance of the <see cref="CommandParameter"/> with the proper type, name and value.
    /// </summary>
    /// <param name="queryGroupTypeMaps">The list of <see cref="QueryGroupTypeMap"/> objects to be converted.</param>
    /// <param name="connection"></param>
    /// <returns>An instance of an object that contains all the definition of the converted underlying <see cref="QueryFields"/>s.</returns>
    /// <param name="transaction"></param><param name="fixParameters">A boolean value whether to fix the parameter name before converting.</param>
    internal static async ValueTask<object> AsMappedObjectAsync(IReadOnlyList<QueryGroupTypeMap> queryGroupTypeMaps,
        IDbConnection connection, IDbTransaction? transaction, string? tableName, CancellationToken cancellationToken = default)
    {
        var dictionary = new ExpandoObject() as IDictionary<string, object?>;

        foreach (var queryGroupTypeMap in queryGroupTypeMaps)
        {
            await AsMappedObjectAsync(dictionary, queryGroupTypeMap, tableName, connection, transaction, cancellationToken).ConfigureAwait(false);
        }

        return (ExpandoObject)dictionary;
    }

    private static void AsMappedObject(IDictionary<string, object?> dictionary,
        in QueryGroupTypeMap queryGroupTypeMap,
        string? tableName,
        IDbConnection connection,
        IDbTransaction? transaction = null)
    {
        var queryFields = queryGroupTypeMap
            .QueryGroup?
            .GetFields(true);

        // Identify if there are fields to count
        if (queryFields?.Any() != true)
        {
            return;
        }

        // Fix the variables for the parameters
        if (tableName is { })
        {
            queryGroupTypeMap.QueryGroup?.Fix(connection, transaction, tableName);
        }

        // Iterate all the query fields
        AsMappedObjectForQueryFields(dictionary, queryGroupTypeMap, queryFields, connection, transaction);
    }

    private static async ValueTask<object> AsMappedObjectAsync(
        IDictionary<string, object?> dictionary,
        QueryGroupTypeMap queryGroupTypeMap,
        string? tableName,
        IDbConnection connection,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        var queryFields = queryGroupTypeMap
            .QueryGroup?
            .GetFields(true);

        // Identify if there are fields to count
        if (queryFields?.Any() != true)
        {
            return new();
        }

        // Fix the variables for the parameters
        if (tableName is { }
            && queryGroupTypeMap.QueryGroup is { })
        {
            await queryGroupTypeMap.QueryGroup.FixAsync(connection, transaction, tableName, cancellationToken).ConfigureAwait(false);
        }

        // Iterate all the query fields
        AsMappedObjectForQueryFields(dictionary, queryGroupTypeMap, queryFields, connection, transaction);
        return new();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="queryGroupTypeMap"></param>
    /// <param name="queryFields"></param>
    private static void AsMappedObjectForQueryFields(IDictionary<string, object?> dictionary,
        in QueryGroupTypeMap queryGroupTypeMap,
        IEnumerable<QueryField> queryFields,
        IDbConnection connection,
        IDbTransaction? transaction)
    {
        foreach (var queryField in queryFields)
        {
            if (queryField.Operation is Operation.Between or Operation.NotBetween)
            {
                AsMappedObjectForBetweenQueryField(dictionary, queryGroupTypeMap, queryField);
            }
            else if (queryField.Operation is Operation.In or Operation.NotIn)
            {
                AsMappedObjectForInQueryField(dictionary, queryGroupTypeMap, queryField, connection, transaction);
            }
            else if (queryField.Operation is Operation.IsNotNull or Operation.IsNull)
            {
                // No params needed
            }
            else
            {
                AsMappedObjectForNormalQueryField(dictionary, queryGroupTypeMap, queryField);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="queryGroupTypeMap"></param>
    /// <param name="queryField"></param>
    private static void AsMappedObjectForBetweenQueryField(IDictionary<string, object?> dictionary,
        in QueryGroupTypeMap queryGroupTypeMap,
        QueryField queryField)
    {
        var values = GetValueList(queryField.Parameter.Value);

        // Left
        var left = string.Concat(queryField.Parameter.Name, "_Left");
        if (!dictionary.ContainsKey(left))
        {
            var leftValue = values.Count > 0 ? values[0] : null;
            if (queryGroupTypeMap.MappedType != null)
            {
                dictionary.Add(left,
                    new CommandParameter(queryField.Field, leftValue, queryGroupTypeMap.MappedType));
            }
            else
            {
                dictionary.Add(left, leftValue);
            }
        }

        // Right
        var right = string.Concat(queryField.Parameter.Name, "_Right");
        if (!dictionary.ContainsKey(right))
        {
            var rightValue = values.Count > 1 ? values[1] : null;
            if (queryGroupTypeMap.MappedType != null)
            {
                dictionary.Add(right,
                    new CommandParameter(queryField.Field, rightValue, queryGroupTypeMap.MappedType));
            }
            else
            {
                dictionary.Add(right, rightValue);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="queryGroupTypeMap"></param>
    /// <param name="queryField"></param>
    private static void AsMappedObjectForInQueryField(IDictionary<string, object?> dictionary,
        in QueryGroupTypeMap queryGroupTypeMap,
        QueryField queryField,
        IDbConnection connection,
        IDbTransaction? transaction)
    {
        if (!queryField.TableParameterMode)
        {
            var values = GetValueList(queryField.Parameter.Value);

            int i;

            for (i = 0; i < values.Count; i++)
            {
                var parameterName = string.Concat(queryField.Parameter.Name, "_In_", i.ToString(CultureInfo.InvariantCulture));
                if (dictionary.ContainsKey(parameterName))
                {
                    continue;
                }

                if (queryGroupTypeMap.MappedType != null)
                {
                    dictionary.Add(parameterName,
                        new CommandParameter(queryField.Field, values[i], queryGroupTypeMap.MappedType));
                }
                else
                {
                    dictionary.Add(parameterName, values[i]);
                }
            }

            var mp = QueryField.RoundUpInLength(i);

            while (i < mp)
            {
                var parameterName = string.Concat(queryField.Parameter.Name, "_In_", i.ToString(CultureInfo.InvariantCulture));

                if (queryGroupTypeMap.MappedType != null)
                {
                    dictionary.Add(parameterName,
                        new CommandParameter(queryField.Field, null, queryGroupTypeMap.MappedType));
                }
                else
                {
                    dictionary.Add(parameterName, null);
                }
                i++;
            }
        }
        else
        {
            var parameterName = queryField.Parameter.Name + "_In_";
            if (!dictionary.ContainsKey(parameterName))
                dictionary.Add(parameterName,
                    connection.GetDbHelper().CreateTableParameter(
                        connection,
                        transaction,
                        queryField.Field.Type,
                        (IEnumerable)queryField.Parameter.Value!,
                        queryField.Parameter.Name));
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="queryGroupTypeMap"></param>
    /// <param name="queryField"></param>
    private static void AsMappedObjectForNormalQueryField(IDictionary<string, object?> dictionary,
        in QueryGroupTypeMap queryGroupTypeMap,
        QueryField queryField)
    {
        if (dictionary.ContainsKey(queryField.Parameter.Name))
        {
            return;
        }

        if (queryGroupTypeMap.MappedType != null)
        {
            dictionary.Add(queryField.Parameter.Name,
                new CommandParameter(queryField.Field, queryField.Parameter.Value, queryGroupTypeMap.MappedType));
        }
        else
        {
            dictionary.Add(queryField.Parameter.Name, queryField.Parameter.Value);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    private static List<T> GetValueList<T>(T value)
    {
        var list = new List<T>();

        if (value is IEnumerable<T> enumerableT)
        {
            list.AddRange(enumerableT);
        }
        else if (value is IEnumerable enumerable)
        {
            var items = enumerable
                .WithType<T>()
                .AsList();
            list.AddRange(items);
        }
        else
        {
            list.AddIfNotNull(value);
        }

        return list;
    }
}
