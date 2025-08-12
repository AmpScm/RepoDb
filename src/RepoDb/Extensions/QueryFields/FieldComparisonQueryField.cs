using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions.QueryFields;

/// <summary>
/// Represents a comparison between two fields (columns) in a query expression.
/// </summary>
public sealed class FieldComparisonQueryField : ExpressionQueryField
{
    public Field Left { get; }
    public Field Right { get; }
    public FieldComparisonQueryField(Field left, Operation operation, Field right)
        : base([left, right], operation)
    {
        Left = left;
        Right = right;
    }

    public override string GetString(IDbSetting dbSetting)
    {
        // Example: [Left] = [Right]
        return $"{Left.FieldName.AsField(dbSetting)} {Operation.GetText()} {Right.FieldName.AsField(dbSetting)}";
    }
}
