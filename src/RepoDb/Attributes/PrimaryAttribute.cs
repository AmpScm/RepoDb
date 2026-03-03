namespace RepoDb.Attributes;

/// <summary>
/// An attribute that is used to define a primary property for the data entity object.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PrimaryAttribute : Attribute
{
    internal static readonly PrimaryAttribute Instance = new();
}
