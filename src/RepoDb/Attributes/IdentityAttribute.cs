namespace RepoDb.Attributes;

/// <summary>
/// An attribute that is used to define an identity property for the data entity object.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IdentityAttribute : Attribute
{
    internal static readonly IdentityAttribute Instance = new();
}
