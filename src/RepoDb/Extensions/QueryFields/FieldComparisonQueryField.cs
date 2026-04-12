using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions.QueryFields;

/// <summary>
/// Represents a comparison between two fields (columns) in a query expression.
/// </summary>
public sealed class FieldComparisonQueryField : ExpressionQueryField
{
    /// <summary>
    ///
    /// </summary>
    public Field Left { get; }
    /// <summary>
    ///
    /// </summary>
    public Field Right { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="left"></param>
    /// <param name="operation"></param>
    /// <param name="right"></param>
    public FieldComparisonQueryField(Field left, Operation operation, Field right)
        : base([left, right], operation)
    {
        Left = left;
        Right = right;
    }

    /// <inheritdoc/>>
    public override string GetString(IDbSetting? dbSetting)
    {
        // Example: [Left] = [Right]
        return $"{Left.FieldName.AsField(dbSetting)} {Operation.GetText()} {Right.FieldName.AsField(dbSetting)}";
    }

    /// <inheritdoc/>>
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Left, Right);
    }

    /// <inheritdoc/>>
    public override bool Equals(QueryField? other)
    {
        return other is FieldComparisonQueryField fqf
            && fqf.Left == Left
            && fqf.Right == Right;
    }
}
