using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;

namespace RepoDb.Reflection;

partial class Compiler
{
    #region PropertyValueAttribute

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbParameterExpression"></param>
    /// <param name="classProperty"></param>
    /// <returns></returns>
    private static List<Expression> GetPropertyValueAttributeAssignmentExpressions(
        ParameterExpression dbParameterExpression,
        ClassProperty classProperty) =>
        GetParameterPropertyValueSetterAttributesAssignmentExpressions((Expression)dbParameterExpression, classProperty);

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbParameterExpression"></param>
    /// <param name="classProperty"></param>
    /// <returns></returns>
    private static List<Expression> GetParameterPropertyValueSetterAttributesAssignmentExpressions(
        Expression dbParameterExpression,
        ClassProperty classProperty)
    {
        var attributes = classProperty?.GetPropertyValueAttributes();
        if (attributes?.Any() != true)
        {
            return [];
        }

        var expressions = new List<Expression>();

        foreach (var attribute in attributes)
        {
            var exclude = !attribute.IncludedInCompilation ||
                string.Equals(nameof(IDbDataParameter.ParameterName), attribute.PropertyName, StringComparison.OrdinalIgnoreCase);

            if (exclude)
            {
                continue;
            }

            var expression = GetPropertyValueAttributesAssignmentExpression(dbParameterExpression,
                attribute);
            expressions.AddIfNotNull(expression);
        }

        return expressions;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbParameterExpression"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    private static MethodCallExpression GetPropertyValueAttributesAssignmentExpression(
        ParameterExpression dbParameterExpression,
        PropertyValueAttribute attribute) =>
        GetPropertyValueAttributesAssignmentExpression((Expression)dbParameterExpression, attribute);

    /// <summary>
    ///
    /// </summary>
    /// <param name="parameterExpression"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    private static MethodCallExpression GetPropertyValueAttributesAssignmentExpression(
        Expression parameterExpression,
        PropertyValueAttribute attribute)
    {
#if NET
        ArgumentNullException.ThrowIfNull(attribute);
#else
        if (attribute == null)
        {
            throw new ArgumentNullException(nameof(attribute));
        }
#endif

        // The problem to this is because of the possibilities of multiple attributes configured for
        // DB multiple providers within a single entity and if the parameterExpression is not really
        // covertible to the target attriute.ParameterType

        //return Expression.Call(Expression.Convert(parameterExpression, attribute.ParameterType),
        //    attribute.PropertyInfo.SetMethod,
        //    Expression.Constant(attribute.Value));

        var method = GetPropertyValueAttributeSetValueMethod();
        return Expression.Call(Expression.Constant(attribute), method, parameterExpression);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private static MethodInfo GetPropertyValueAttributeSetValueMethod() =>

        StaticType.PropertyValueAttribute.GetMethod(nameof(PropertyValueAttribute.SetValue),
            BindingFlags.Instance | BindingFlags.NonPublic);

    #endregion
}
