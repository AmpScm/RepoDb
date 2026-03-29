using System.Data;
using System.Linq.Expressions;
using RepoDb.DbSettings;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Reflection;

internal partial class Compiler
{
    private static BlockExpression GetDataEntityParameterAssignmentExpression(ParameterExpression dbCommandExpression,
        int entityIndex,
        Expression entityExpression,
        ParameterExpression propertyExpression,
        DbField dbField,
        ClassProperty? classProperty,
        ParameterDirection direction,
        IDbSetting dbSetting,
        IDbHelper? dbHelper)
    {
        var assignmentExpressions = new List<Expression>(16);
        var dbParameterExpression = Expression.Variable(StaticType.DbParameter,
            string.Concat("parameter", dbField.FieldName.AsAlphaNumeric()));


        // Variable
        var createParameterExpression = GetDbCommandCreateParameterExpression(dbCommandExpression);
        assignmentExpressions.AddIfNotNull(Expression.Assign(dbParameterExpression, createParameterExpression));

        // DbParameter.ParameterName
        var nameAssignmentExpression = GetDbParameterNameAssignmentExpression(dbParameterExpression,
            dbField,
            entityIndex,
            dbSetting);
        assignmentExpressions.AddIfNotNull(nameAssignmentExpression);

        // DbParameter.Value
        if (direction is not ParameterDirection.Output)
        {
            var valueAssignmentExpression = GetDataEntityDbParameterValueAssignmentExpression(dbParameterExpression,
                entityExpression,
                propertyExpression,
                classProperty,
                dbField,
                dbCommandExpression);
            assignmentExpressions.AddIfNotNull(valueAssignmentExpression);
        }

        // DbParameter.DbType
        if (GetDbType(classProperty, dbField) is { } dbType)
        {
            assignmentExpressions.Add(GetDbParameterDbTypeAssignmentExpression(dbParameterExpression, dbType));
        }

        // DbParameter.Direction
        if (dbSetting.IsDirectionSupported && direction is not ParameterDirection.Input and not 0)
        {
            assignmentExpressions.Add(GetDbParameterDirectionAssignmentExpression(dbParameterExpression, direction));
        }

        // DbParameter.Size
        if (dbField.Size is { } size)
        {
            assignmentExpressions.Add(GetDbParameterSizeAssignmentExpression(dbParameterExpression, size));
        }

        // DbParameter.Precision
        if (dbField.Precision is { } precision)
        {
            assignmentExpressions.Add(GetDbParameterPrecisionAssignmentExpression(dbParameterExpression, precision));
        }

        // DbParameter.Scale
        if (dbField.Scale is { } scale)
        {
            assignmentExpressions.Add(GetDbParameterScaleAssignmentExpression(dbParameterExpression, scale));
        }

        // Compiler.DbParameterPostCreation
        var dbParameterPostCreationExpression =
            (dbHelper as BaseDbHelper)?.GetParameterPostCreationExpression(dbParameterExpression, propertyExpression, dbField);
        assignmentExpressions.AddIfNotNull(dbParameterPostCreationExpression);

        // PropertyValueAttributes / DbField must precide
        var propertyValueAttributeAssignmentExpressions = GetParameterPropertyValueSetterAttributesAssignmentExpressions(dbParameterExpression, classProperty);
        assignmentExpressions.AddRangeIfNotNullOrNotEmpty(propertyValueAttributeAssignmentExpressions);

        // DbCommand.Parameters.Add
        var dbParametersAddExpression = GetDbCommandParametersAddExpression(dbCommandExpression, dbParameterExpression);
        assignmentExpressions.AddIfNotNull(dbParametersAddExpression);

        // Return the value
        return Expression.Block([dbParameterExpression], assignmentExpressions);
    }
}
