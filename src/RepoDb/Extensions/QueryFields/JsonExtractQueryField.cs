using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions.QueryFields;

/// <summary>
/// A functional-based <see cref="QueryField"/> object that is using the LENGTH function.
/// This only works on PostgreSQL, MySQL and SQLite database providers.
/// </summary>
public sealed partial class JsonExtractQueryField : FunctionalQueryField
{
    public static readonly string JsonExtractFormat = "JSON_EXTRACT({0}, @@path@@)";

    #region Constructors

    /// <summary>
    /// Creates a new instance of <see cref="LengthQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="path"></param>
    /// <param name="value">The value to be used for the query expression.</param>
    public JsonExtractQueryField(
        string fieldName,
        string path,
        object? value)
        : this(fieldName, path, Operation.Equal, value, null)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="LengthQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="path"></param>
    /// <param name="value">The value to be used for the query expression.</param>
    /// <param name="dbType">The database type to be used for the query expression.</param>
    public JsonExtractQueryField(string fieldName,
        string path,
        object? value,
        DbType? dbType)
        : this(fieldName, path, Operation.Equal, value, dbType)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="LengthQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="path"></param>
    /// <param name="operation">The operation to be used for the query expression.</param>
    /// <param name="value">The value to be used for the query expression.</param>
    public JsonExtractQueryField(string fieldName,
        string path,
        Operation operation,
        object? value)
        : this(fieldName, path, operation, value, null)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="LengthQueryField"/> object.
    /// </summary>
    /// <param name="fieldName">The name of the field for the query expression.</param>
    /// <param name="path"></param>
    /// <param name="operation">The operation to be used for the query expression.</param>
    /// <param name="value">The value to be used for the query expression.</param>
    /// <param name="dbType">The database type to be used for the query expression.</param>
    public JsonExtractQueryField(string fieldName,
        string path,
        Operation operation,
        object? value,
        DbType? dbType)
        : base(fieldName, operation, value, dbType ?? DbType.Int32, JsonExtractFormat)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        Path = path;
    }

    public string Path { get; }


    public override string GetString(int index, IDbSetting? dbSetting)
    {
        if (dbSetting is BaseDbSetting bs
            && bs.CreateJsonExtract(Path, Parameter) is string v)
        {
            return GetString(index, v, dbSetting);
        }
        else
            return base.GetString(index, dbSetting).Replace("@@path@@", string.Concat("'", ToJsonPath(Path).Replace("'", "''"), "'"));
    }

    #endregion

    public override bool Equals(QueryField? other)
    {
        return other is JsonExtractQueryField jef
            && base.Equals(jef)
            && Path == jef.Path;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Path);
    }


#if NET9_0_OR_GREATER
    [GeneratedRegex(@"([a-zA-Z0-9_]+)|\[(\d+)\]", RegexOptions.Compiled)]
    private static partial
#else
    private static
#endif
    Regex SplitPathRegex
    { get; }
#if !NET9_0_OR_GREATER
        = new Regex(@"([a-zA-Z0-9_]+)|\[(\d+)\]", RegexOptions.Compiled);
#endif


    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitJsonPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            yield break;

        foreach (Match m in SplitPathRegex.Matches(path))
        {
            if (m.Groups[1].Success)
                yield return m.Groups[1].Value;       // property
            else if (m.Groups[2].Success)
                yield return $"[{m.Groups[2].Value}]"; // array index
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ToJsonPath(string path)
    {
        var segments = SplitJsonPath(path);
        var sb = new StringBuilder("$");

        foreach (var seg in segments)
        {
            if (seg[0] == '[')
            {
                sb.Append(seg); // array index
            }
            else
            {
                sb.Append('.').Append(seg); // property
            }
        }

        return sb.ToString();
    }

    internal static string ParsePath(Expression path)
    {
        if (path is not LambdaExpression lambda)
            throw new ArgumentOutOfRangeException(nameof(path));

        StringBuilder sb = new();

        var r = lambda.Body;

        AppendToPath(r);


        return sb.ToString();

        void AppendToPath(Expression? e)
        {
            if (e is null)
                return;

            if (e is MemberExpression me)
            {
                AppendToPath(me.Expression);
                AppendName(me.Member.Name);
            }
            else if (e is BinaryExpression be && be.NodeType == ExpressionType.ArrayIndex)
            {
                AppendToPath(be.Left);

                if (be.Right.GetValue() is int ix)
                {
#if NET
                    sb.Append(CultureInfo.InvariantCulture, $"[{ix}]");
#else
                    sb.AppendFormat(CultureInfo.InvariantCulture, "[{0}]", ix);
#endif
                }
            }
            else if (e is ParameterExpression)
            {
                return;
            }
            else
                throw new InvalidOperationException($"Unexpected node of type {e.NodeType}");
        }
        void AppendName(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (Converter.JsonSerializerOptions.PropertyNamingPolicy is {} cn)
                name = cn.ConvertName(name);

            if (sb.Length > 0)
                sb.Append('.');
            sb.Append(name);
        }
    }
}
