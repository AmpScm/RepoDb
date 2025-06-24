namespace RepoDb.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class IgnoreAttribute : Attribute
{
}
