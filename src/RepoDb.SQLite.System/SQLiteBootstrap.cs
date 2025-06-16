using System.Data.SQLite;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;

namespace RepoDb;

/// <summary>
/// A class that is being used to initialize necessary objects that is connected to <see cref="SQLiteConnection"/> object.
/// </summary>
public static class SQLiteBootstrap
{
    #region Properties

    /// <summary>
    /// Gets the value indicating whether the initialization is completed.
    /// </summary>
    public static bool IsInitialized { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes all necessary settings for SqLite.
    /// </summary>
    [Obsolete("This class will soon to be hidden as internal class. Use the 'GlobalConfiguration.Setup().UseSQLite()' method instead.")]
    public static void Initialize() => InitializeInternal();

    /// <summary>
    ///
    /// </summary>
    internal static void InitializeInternal()
    {
        // Skip if already initialized
        if (IsInitialized == true)
        {
            return;
        }

        #region SDS

        // Map the DbSetting
        var sdsDbSetting = new SQLiteDbSetting(true);
        DbSettingMapper.Add<SQLiteConnection>(sdsDbSetting, true);

        // Map the DbHelper
        DbHelperMapper.Add<SQLiteConnection>(new SQLiteDbHelper(sdsDbSetting, new SqLiteDbTypeNameToClientTypeResolver()), true);

        // Map the Statement Builder
        StatementBuilderMapper.Add<SQLiteConnection>(new SQLiteStatementBuilder(sdsDbSetting,
            new SqLiteConvertFieldResolver(),
            new ClientTypeToAverageableClientTypeResolver()), true);

        #endregion

        // Set the flag
        IsInitialized = true;
    }

    #endregion
}
