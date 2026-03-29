namespace RepoDb.Enumerations;

/// <summary>
/// An enumeration that is used to define an operation on the query expression.
/// </summary>
public enum Operation
{
    /// <summary>
    /// An equal operation.
    /// </summary>
    Equal = 1968935258,
    /// <summary>
    /// A not-equal operation.
    /// </summary>
    NotEqual = 1861928312,
    /// <summary>
    /// A less-than operation.
    /// </summary>
    LessThan = 1185489222,
    /// <summary>
    /// A greater-than operation.
    /// </summary>
    GreaterThan = 1904766133,
    /// <summary>
    /// A less-than-or-equal operation.
    /// </summary>
    LessThanOrEqual = 1969886037,
    /// <summary>
    /// A greater-than-or-equal operation.
    /// </summary>
    GreaterThanOrEqual = 1388583607,
    /// <summary>
    /// A like operation. Defines the (LIKE) keyword in SQL Statement.
    /// </summary>
    Like = 761684150,
    /// <summary>
    /// A not-like operation. Defines the (NOT LIKE) keyword in SQL Statement.
    /// </summary>
    NotLike = 9672391,
    /// <summary>
    /// A between operation. Defines the (BETWEEN) keyword in SQL Statement.
    /// </summary>
    Between = 1639933206,
    /// <summary>
    /// A not-between operation. Defines the (NOT BETWEEN) keyword in SQL Statement.
    /// </summary>
    NotBetween = 729234306,
    /// <summary>
    /// An in operation. Defines the (IN) keyword in SQL Statement.
    /// </summary>
    In = 1986985380,
    /// <summary>
    /// A not-in operation. Defines the (NOT IN) keyword in SQL Statement.
    /// </summary>
    NotIn = 277789918,
    /// <summary>
    /// Checks if the column is null
    /// </summary>
    IsNull = 198699918,
    /// <summary>
    /// Checks if the column is not null
    /// </summary>
    IsNotNull = 761683607,
}

internal static class OperationExtensions
{
    public static Operation SwitchOperands(this Operation operation)
         => operation switch
         {
             Operation.Equal => Operation.Equal,
             Operation.NotEqual => Operation.NotEqual,
             Operation.LessThan => Operation.GreaterThan,
             Operation.GreaterThan => Operation.LessThan,
             Operation.LessThanOrEqual => Operation.GreaterThanOrEqual,
             Operation.GreaterThanOrEqual => Operation.LessThanOrEqual,
             _ => throw new NotSupportedException($"The operation '{operation}' doesn't support the switching of operands.")
         };
}
