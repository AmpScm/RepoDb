using NpgsqlTypes;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// A class that is being used to resolve the .NET CLR Type into its equivalent <see cref="NpgsqlDbType"/>.
/// </summary>
public class ClientTypeToNpgsqlDbTypeResolver : IResolver<Type, NpgsqlDbType?>
{
    /// <summary>
    /// Returns the equivalent <see cref="NpgsqlDbType"/> based from the .NET CLR Type.
    /// </summary>
    /// <param name="type">The target .NET CLR type.</param>
    /// <returns>The equivalent <see cref="NpgsqlDbType"/>.</returns>
    public virtual NpgsqlDbType? Resolve(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type == typeof(NpgsqlBox))
        {
            return NpgsqlDbType.Box;
        }
        else if (type == typeof(NpgsqlCircle))
        {
            return NpgsqlDbType.Circle;
        }
        else if (type == typeof(NpgsqlLine))
        {
            return NpgsqlDbType.Line;
        }
        else if (type == typeof(NpgsqlLogSequenceNumber))
        {
            return NpgsqlDbType.PgLsn;
        }
        else if (type == typeof(NpgsqlLSeg))
        {
            return NpgsqlDbType.LSeg;
        }
        else if (type == typeof(NpgsqlPath))
        {
            return NpgsqlDbType.Path;
        }
        else if (type == typeof(NpgsqlPoint))
        {
            return NpgsqlDbType.Point;
        }
        else if (type == typeof(NpgsqlPolygon))
        {
            return NpgsqlDbType.Polygon;
        }
        else if (type == typeof(NpgsqlRange<DateTime>) ||
            type == typeof(NpgsqlRange<decimal>) ||
            type == typeof(NpgsqlRange<int>) ||
            type == typeof(NpgsqlRange<long>))
        {
            return NpgsqlDbType.Unknown;
        }
        else if (type == typeof(NpgsqlTid))
        {
            return NpgsqlDbType.Tid;
        }
        else if (type == typeof(NpgsqlTsQuery))
        {
            return NpgsqlDbType.TsQuery;
        }
        else if (type == typeof(NpgsqlTsVector))
        {
            return NpgsqlDbType.TsVector;
        }
        else if (type == typeof(Array))
        {
            return NpgsqlDbType.Unknown;
        }
        else if (type == typeof(bool))
        {
            return NpgsqlDbType.Boolean;
        }
        else if (type == typeof(byte[]))
        {
            return NpgsqlDbType.Bytea;
        }
        else if (type == typeof(char))
        {
            return NpgsqlDbType.InternalChar;
        }
        else if (type == typeof(System.Collections.BitArray))
        {
            return NpgsqlDbType.Bit;
        }
        else if (type == typeof(DateTime))
        {
            return NpgsqlDbType.Timestamp;
        }
        else if (type == typeof(DateTimeOffset))
        {
            return NpgsqlDbType.TimestampTz;
        }
#if NET
        else if (type == typeof(DateOnly))
        {
            return NpgsqlDbType.Date;
        }
        else if (type == typeof(TimeOnly))
        {
            return NpgsqlDbType.Time;
        }
#endif
        else if (type == typeof(decimal))
        {
            return NpgsqlDbType.Money;
        }
        else if (type == typeof(double))
        {
            return NpgsqlDbType.Double;
        }
        else if (type == typeof(Guid))
        {
            return NpgsqlDbType.Uuid;
        }
        else if (type == typeof(short))
        {
            return NpgsqlDbType.Smallint;
        }
        else if (type == typeof(int))
        {
            return NpgsqlDbType.Integer;
        }
        else if (type == typeof(long))
        {
            return NpgsqlDbType.Bigint;
        }
        else if (type == typeof(System.Net.IPAddress))
        {
            return NpgsqlDbType.Inet;
        }
        else if (type == typeof(System.Net.NetworkInformation.PhysicalAddress))
        {
            return NpgsqlDbType.MacAddr;
        }
        else if (type == typeof(float))
        {
            return NpgsqlDbType.Real;
        }
        else if (type == typeof(string))
        {
            return NpgsqlDbType.Char;
        }
        else if (type == typeof(TimeSpan))
        {
            return NpgsqlDbType.Interval;
        }
        else if (type == typeof(uint))
        {
            return NpgsqlDbType.Cid;
        }
        else if (type == typeof(ValueTuple<System.Net.IPAddress, int>))
        {
            return NpgsqlDbType.Cidr;
        }

        throw new InvalidOperationException($"The type '{type.FullName}' could not be resolved to '{typeof(NpgsqlDbType).FullName}'.");
    }

    /// <summary>
    /// The default instance of the <see cref="ClientTypeToNpgsqlDbTypeResolver"/> class.
    /// </summary>
    public static readonly ClientTypeToNpgsqlDbTypeResolver Instance = new();
}
