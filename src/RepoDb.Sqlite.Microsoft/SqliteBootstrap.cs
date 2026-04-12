using Microsoft.Data.Sqlite;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;

namespace RepoDb;

/// <summary>
/// A class that is being used to initialize necessary objects that is connected to <see cref="SqliteConnection"/> object.
/// </summary>
public static class SqliteBootstrap
{
    #region Properties

    /// <summary>
    /// Gets the value indicating whether the initialization is completed.
    /// </summary>
    public static bool IsInitialized { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes all necessary settings for Sqlite.
    /// </summary>
    [Obsolete("This class will soon to be hidden as internal class. Use the 'GlobalConfiguration.Setup().UseSqlite()' method instead.")]
    public static void Initialize() => InitializeInternal();

    internal static void InitializeInternal()
    {
        // Skip if already initialized
        if (IsInitialized)
        {
            return;
        }

        // Map the DbSetting
        var setting = new SqliteDbSetting();
        DbSettingMapper.Add<SqliteConnection>(setting, true);

        // Map the DbHelper
        DbHelperMapper.Add<SqliteConnection>(new SqliteDbHelper(setting), true);

        // Map the Statement Builder
        StatementBuilderMapper.Add<SqliteConnection>(new SqliteStatementBuilder(setting), true);

        // Set the flag
        IsInitialized = true;
    }

    #endregion
}
