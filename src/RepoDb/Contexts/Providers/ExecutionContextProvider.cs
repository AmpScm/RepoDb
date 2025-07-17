using RepoDb.Enumerations;

namespace RepoDb.Contexts.Providers;

/// <summary>
///
/// </summary>
internal static class ExecutionContextProvider
{
    #region KeyColumnReturnBehavior

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <returns></returns>
    public static Field? GetTargetReturnColumnAsField(Type entityType,
        DbFieldCollection dbFields)
    {
        var primaryField = GetPrimaryAsReturnKeyField(entityType, dbFields);
        var identityField = GetIdentityAsReturnKeyField(entityType, dbFields);

        return GlobalConfiguration.Options.KeyColumnReturnBehavior switch
        {
            KeyColumnReturnBehavior.Primary => primaryField,
            KeyColumnReturnBehavior.Identity => identityField,
            KeyColumnReturnBehavior.PrimaryOrElseIdentity => primaryField ?? identityField,
            KeyColumnReturnBehavior.IdentityOrElsePrimary => identityField ?? primaryField,
            _ => throw new InvalidOperationException(nameof(GlobalConfiguration.Options.KeyColumnReturnBehavior)),
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Field? GetPrimaryAsReturnKeyField(Type entityType,
        DbFieldCollection dbFields) =>
        PrimaryCache.Get(entityType)?.AsField() ??
            dbFields?.GetPrimary();

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Field? GetIdentityAsReturnKeyField(Type entityType,
        DbFieldCollection dbFields) =>
        IdentityCache.Get(entityType)?.AsField() ??
            dbFields?.GetIdentity();

    #endregion
}
