using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb;

/// <summary>
/// A class that is being used to initialize the necessary settings for the <see cref="SqlConnection"/> object.
/// </summary>
public static class SqlServerBootstrap
{
    #region Properties

    /// <summary>
    /// Gets the value that indicates whether the initialization is completed.
    /// </summary>
    public static bool IsInitialized { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes all the necessary settings for SQL Server.
    /// </summary>
    [Obsolete("This class will soon to be hidden as internal class. Use the 'GlobalConfiguration.Setup().UseSqlServer()' method instead.")]
    public static void Initialize() => InitializeInternal();

    internal static void InitializeInternal()
    {
        // Skip if already initialized
        if (IsInitialized)
        {
            return;
        }

        // Map the DbSetting
        var dbSetting = new SqlServerDbSetting();
        DbSettingMapper.Add<SqlConnection>(dbSetting, true);

        // Map the DbHelper
        var dbHelper = new SqlServerDbHelper();
        DbHelperMapper.Add<SqlConnection>(dbHelper, true);

        // Map the Statement Builder
        var statementBuilder = new SqlServerStatementBuilder(dbSetting);
        StatementBuilderMapper.Add<SqlConnection>(statementBuilder, true);

        // Set the flag
        IsInitialized = true;
    }

    internal static void InitializeSystemDataSqlClient()
    {
        foreach(var className in new string[]
        {
            "System.Data.SqlClient.SqlConnection, System.Data.SqlClient",
            "System.Data.SqlClient.SqlConnection, System.Data"
        })
        {
            if (Type.GetType(className, false) is { } connectionType)
            {
                var dbSetting = new SqlServerDbSetting();
                var dbhelper = new SqlServerDbHelper();
                var statementBuilder = new SqlServerStatementBuilder(dbSetting);

                Expression<Action> addSetting = () => DbSettingMapper.Add<SqlConnection>(dbSetting, true);
                Expression<Action> addHelper = () => DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
                Expression<Action> addStatementBuilder = () => StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(dbSetting), true);

                ((MethodCallExpression)addSetting.Body).Method.GetGenericMethodDefinition().MakeGenericMethod(connectionType).Invoke(null, [dbSetting, true]);
                ((MethodCallExpression)addHelper.Body).Method.GetGenericMethodDefinition().MakeGenericMethod(connectionType).Invoke(null, [dbhelper, true]);
                ((MethodCallExpression)addStatementBuilder.Body).Method.GetGenericMethodDefinition().MakeGenericMethod(connectionType).Invoke(null, [statementBuilder, true]);
            }
        }
    }

    #endregion
}
