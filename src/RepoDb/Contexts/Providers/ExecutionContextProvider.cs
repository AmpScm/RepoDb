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
        var primaryField = PrimaryCache.Get(entityType)?.AsField() ?? dbFields?.GetPrimary();
        var identityField = IdentityCache.Get(entityType)?.AsField() ?? dbFields?.Identity;

        return GlobalConfiguration.Options.KeyColumnReturnBehavior switch
        {
            KeyColumnReturnBehavior.Primary => primaryField,
            KeyColumnReturnBehavior.Identity => identityField,
            KeyColumnReturnBehavior.PrimaryOrElseIdentity => primaryField ?? identityField,
            KeyColumnReturnBehavior.IdentityOrElsePrimary => identityField ?? primaryField,
            _ => throw new InvalidOperationException(nameof(GlobalConfiguration.Options.KeyColumnReturnBehavior)),
        };
    }

    #endregion
}
