using System;
using System.Collections.Generic;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers;

/// <summary>
/// 
/// </summary>
public sealed class ClientTypeToOracleDbTypeResolver : IResolver<Type, OracleDbType?>
{
    /// <inheritdoc/>
    public OracleDbType? Resolve(Type input)
    {
        if (input == typeof(string))
            return OracleDbType.Varchar2; //
        else if (input == typeof(long) || input == typeof(ulong))
            return OracleDbType.Int64;
        else if (input == typeof(int) || input == typeof(uint))
            return OracleDbType.Int32;
        else if (input == typeof(short) || input == typeof(ushort))
            return OracleDbType.Int16;
        else if (input == typeof(byte) || input == typeof(sbyte))
            return OracleDbType.Byte;
        else if (input == typeof(char))
            return OracleDbType.Char;
        else if (input == typeof(decimal))
            return OracleDbType.Decimal;
        else if (input == typeof(double))
            return OracleDbType.Double;
        else if (input == typeof(float))
            return OracleDbType.Single;
        else if (input == typeof(bool))
            return OracleDbType.Boolean;


        return null;
    }


    /// <summary>
    /// The one instance of <see cref="ClientTypeToOracleDbTypeResolver"/>
    /// </summary>
    public static readonly ClientTypeToOracleDbTypeResolver Instance = new();
}
