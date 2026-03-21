using System.Linq.Expressions;
using RepoDb.Exceptions;

namespace RepoDb.Extensions;

public static partial class ExpressionExtension
{
    #region GetValue


    /// <summary>
    /// Gets a value from the current instance of <see cref="Expression"/> object.
    /// </summary>
    /// <param name="expression">The instance of <see cref="Expression"/> object where the value is to be extracted.</param>
    /// <returns>The extracted value from <see cref="Expression"/> object.</returns>
    [Obsolete("Use TryGetValue(), which allows handling errors")]
    public static object? GetValue(this Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (expression.TryGetValue(out var value))
        {
            return value;
        }
        else
            throw new InvalidExpressionException($"Expression '{expression}'-s value can't be calculated from this expression. Does it use the passed argument?.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetValue(this Expression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);
        value = null;
        return expression switch
        {
            BinaryExpression binaryExpression => binaryExpression.TryGetValue(out value),
            ConstantExpression constantExpression => constantExpression.TryGetValue(out value),
            UnaryExpression unaryExpression => unaryExpression.TryGetValue(out value),
            MethodCallExpression methodCallExpression => methodCallExpression.TryGetValue(out value),
            MemberExpression memberExpression => memberExpression.TryGetValue(out value),
            NewArrayExpression newArrayExpression => newArrayExpression.TryGetValue(out value),
            ListInitExpression listInitExpression => listInitExpression.TryGetValue(out value),
            NewExpression newExpression => newExpression.TryGetValue(out value),
            MemberInitExpression memberInitExpression => memberInitExpression.TryGetValue(out value),
            ConditionalExpression conditionalExpression => conditionalExpression.TryGetValue(out value),
            DefaultExpression defaultExpression => defaultExpression.TryGetValue(out value),
            ParameterExpression => false, // Explicit value. Not deterministic
            _ => false
        };
    }

    internal static bool TryGetValue(this BinaryExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (!expression.Left.TryGetValue(out var left)
            || !expression.Right.TryGetValue(out var right))
        {
            value = null;
            return false;
        }

        // Very limited operation. Should be extended to really use expressions
        switch (expression.NodeType)
        {
            case ExpressionType.Equal:
            case ExpressionType.NotEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
            case ExpressionType.Or:
            case ExpressionType.OrElse:
            case ExpressionType.Coalesce:
            case ExpressionType.ArrayIndex:
            case ExpressionType.Add:
            case ExpressionType.AddChecked:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.ExclusiveOr:
            case ExpressionType.LeftShift:
            case ExpressionType.RightShift:
            case ExpressionType.Power:
                // Interpret the expression by compiling it. This is not the most efficient way, but it works for a wide range of expressions without needing to implement them all manually.
                // And the implementation is correct, as it uses the same semantics as the expression would at runtime.
                value = Expression.Lambda(
                    expression.Replace(expression.Left, Expression.Constant(left, expression.Left.Type)).Replace(expression.Right, Expression.Constant(right, expression.Right.Type))).Compile(true).DynamicInvoke();
                return true;
        }

        value = null;
        return false;
    }
    internal static bool TryGetValue(this ConstantExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);
        value = expression.Value;
        return true;
    }

    internal static bool TryGetValue(this UnaryExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        switch (expression.NodeType)
        {
            case ExpressionType.Quote:
                value = expression.Operand; // The inner expression is the value
                return true;

            case ExpressionType.Not when expression.Operand.TryGetValue(out value):
            case ExpressionType.Convert when expression.Operand.TryGetValue(out value):
            case ExpressionType.Negate when expression.Operand.TryGetValue(out value):
            case ExpressionType.NegateChecked when expression.Operand.TryGetValue(out value):
            case ExpressionType.UnaryPlus when expression.Operand.TryGetValue(out value):
                value = Expression.Lambda(
                    expression.Replace(expression.Operand, Expression.Constant(value, expression.Operand.Type))).Compile(true).DynamicInvoke();
                return true;
        }

        value = null;
        return false;
    }

    internal static bool TryGetValue(this MethodCallExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var args = expression.Arguments.Select(a => a.TryGetValue(out var item) ? new { item } : null).ToArray();

        if (!args.All(x => x is { }))
        {
            value = null;
            return false;
        }

        object? obj = null;
        if (expression.Object is { } && !expression.Object.TryGetValue(out obj))
        {
            value = null;
            return false;
        }

        if (expression.Method is { IsSpecialName: true, Name: "op_Implicit" } method && method.DeclaringType?.IsSpan() == true
            && args.Length == 1)
        {
            value = args[0]!.item;
            return true;
        }
#if NET
        else if (expression.Arguments.Any(x => x.Type.IsByRefLike) || expression.Object?.Type.IsByRefLike == true)
        {
            value = null;
            return false;
        }
#endif

        var defs = expression.Method.GetParameters();
        // If possible replace arguments with their pre-calculated values to avoid double evaluations

        // We can't replace output arguments with their value, as they are not passed as arguments,
        // but as references. So we need to keep them as they are, and only replace the input arguments.
        expression = expression.Update(
            (obj is { } && !expression.Object!.Type.IsSpan()) ? Expression.Constant(obj, expression.Object!.Type) : expression.Object,
            expression.Arguments.Zip(args, defs)
                .Select(v => (v.Third.IsOut || v.First.Type.IsSpan()) ? v.First : Expression.Constant(v.Second!.item!, v.First.Type)));

        value = Expression.Lambda(expression).Compile(true).DynamicInvoke();
        return true;
    }


    internal static bool TryGetValue(this MemberExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        value = null;
        if (expression.Expression?.TryGetValue(out value) != false
            && expression.Member.TryGetValue(value, null, out value))
        {
            return true;
        }

        value = null;
        return false;
    }

    internal static bool TryGetValue(this NewArrayExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var arrayType = expression.Type.HasElementType ? expression.Type.GetElementType()! : expression.Type;
        var array = Array.CreateInstance(arrayType, expression.Expressions.Count);
        for (var i = 0; i < expression.Expressions.Count; i++)
        {
            if (!expression.Expressions[i].TryGetValue(out var elementValue))
            {
                value = null;
                return false;
            }

            array.SetValue(elementValue, i);
        }
        value = array;
        return true;
    }

    internal static bool TryGetValue(this ListInitExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var list = Activator.CreateInstance(expression.Type);
        foreach (var item in expression.Initializers)
        {
            var args = item.Arguments.Select(a => a.TryGetValue(out var item) ? new { item } : null);

            if (!args.All(x => x is { }))
            {
                value = null;
                return false;
            }
            item.AddMethod.Invoke(list, args.Select(x => x!.item).ToArray());
        }
        value = list;
        return true;
    }

    internal static bool TryGetValue(this NewExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (expression.Arguments.Count > 0)
        {
            var args = expression.Arguments.Select(a => a.TryGetValue(out var item) ? new { item } : null);

            if (!args.All(x => x is { }))
            {
                value = null;
                return false;
            }

            value = Activator.CreateInstance(expression.Type, args.Select(x => x!.item).ToArray());
            return true;
        }
        else
        {
            value = Activator.CreateInstance(expression.Type);
            return true;
        }
    }

    internal static bool TryGetValue(this MemberInitExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (!expression.NewExpression.TryGetValue(out value) || value is null)
        {
            value = null;
            return false;
        }

        foreach (var binding in expression.Bindings)
        {
            if (binding is MemberAssignment assignment
                && assignment.Expression.TryGetValue(out var memberValue))
            {
                // new AA() { B = "c" }
                binding.Member.SetValue(value, memberValue);
            }
            else if (binding is MemberListBinding listBinding)
            {
                // new AA() { B = { "a", "b", "c"} } on some collection type member

                if (!TryApplyListBinding(value, listBinding))
                    return false;
            }
            else if (binding is MemberMemberBinding memberMemberBinding)
            {
                // new AA() { B = new() { C = 12 }}
                if (!TryApplyMemberMemberBinding(value, memberMemberBinding))
                    return false;
                break;
            }
            else
            {
                value = null;
                return false;
            }

        }
        return true;

        static bool TryApplyListBinding(object target, MemberListBinding listBinding)
        {
            if (!listBinding.Member.TryGetValue(target, null, out var collection))
                return false;

            if (collection is null)
                return false;

            foreach (var elementInit in listBinding.Initializers)
            {
                var args = new object?[elementInit.Arguments.Count];
                for (int i = 0; i < args.Length; i++)
                {
                    if (!elementInit.Arguments[i].TryGetValue(out var argValue))
                        return false;

                    args[i] = argValue;
                }

                // Voer de Add(...) call uit
                elementInit.AddMethod.Invoke(collection, args);
            }

            return true;
        }

        static bool TryApplyMemberMemberBinding(object target, MemberMemberBinding memberBinding)
        {
            // Haal de bestaande nested instance op, of maak een nieuwe
            if (!memberBinding.Member.TryGetValue(target, null, out var nested))
                return false;

            if (nested is null)
                return false;

            // Verwerk alle nested bindings
            foreach (var binding in memberBinding.Bindings)
            {
                switch (binding)
                {
                    case MemberAssignment assignment:
                        if (!assignment.Expression.TryGetValue(out var assignedValue))
                            return false;

                        binding.Member.SetValue(nested, assignedValue);
                        break;

                    case MemberListBinding listBinding:
                        if (!TryApplyListBinding(nested, listBinding))
                            return false;
                        break;

                    case MemberMemberBinding deeperBinding:
                        if (!TryApplyMemberMemberBinding(nested, deeperBinding))
                            return false;
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }

    }

    internal static bool TryGetValue(this ConditionalExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        // Yes we short-circuit here, to avoid doing expensive things.
        // This moves the testing of both paths to the caller, but it is more efficient.
        if (expression.Test.TryGetValue(out var testValue)
            && testValue is bool useIfTrue
            && (useIfTrue ? expression.IfTrue : expression.IfFalse).TryGetValue(out value))
        {
            return true;
        }

        value = null;
        return false;
    }

    internal static bool TryGetValue(this DefaultExpression expression, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expression);

        value = expression.Type.IsValueType ? Activator.CreateInstance(expression.Type) : null;
        return true;
    }

    #endregion
}
