using System.Reflection;

namespace RepoDb.Extensions;

/// <summary>
/// Contains the extension methods for <see cref="MemberInfo"/> object.
/// </summary>
internal static class MemberInfoExtension
{
    /// <summary>
    /// Gets the name of the current instance of <see cref="MemberInfo"/>. If the instance is <see cref="PropertyInfo"/>, it will try to retrieved the
    /// mapped name of the property.
    /// </summary>
    /// <param name="member">The member where to retrieve a name.</param>
    /// <returns>The name of the <see cref="MemberInfo"/>.</returns>
    internal static string GetMappedName(this MemberInfo member) =>
        member is PropertyInfo memberInfo ? PropertyMappedNameCache.Get(memberInfo) : member.Name;

    internal static bool TryGetValue(this MemberInfo member,
        object? obj,
        object?[]? parameters,
        out object? value)
    {
        if (member is FieldInfo fieldInfo)
        {
            value = fieldInfo.GetValue(obj);
            return true;
        }
        else if (member is PropertyInfo propertyInfo)
        {
            if ((propertyInfo.GetMethod ?? propertyInfo.SetMethod)!.IsStatic != (obj == null))
            {
                if (member.DeclaringType?.IsGenericType == true && member.DeclaringType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (propertyInfo.Name == nameof(Nullable<>.HasValue))
                    {
                        value = obj is not null;
                        return true;
                    }
                    else if (propertyInfo.Name == nameof(Nullable<>.Value))
                    {
                        value = obj;
                        return true;
                    }
                }
                value = null;
                return false;
            }

            value = propertyInfo.GetValue(obj);
            return true;
        }
        else if (member is MethodInfo methodInfo)
        {
            if (methodInfo is { IsSpecialName: true, Name: "op_Implicit" } m && m.DeclaringType?.IsSpan() == true)
            {
                value = parameters?[0];
                return parameters is { Length: 1 };
            }
            else
            {
                value = methodInfo.Invoke(obj, parameters);
                return true;
            }
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Sets the value of an object member based on the retrieved value from the instance of <see cref="MemberInfo"/> object.
    /// </summary>
    /// <param name="member">The instance of <see cref="MemberInfo"/> object where the value is to be retrieved.</param>
    /// <param name="obj">The object whose member value will be set.</param>
    /// <param name="value">The target value of the member.</param>
    internal static void SetValue(this MemberInfo member,
        object? obj,
        object? value)
    {
        if (member is FieldInfo fieldInfo)
        {
            fieldInfo.SetValue(obj, value);
        }
        else if (member is PropertyInfo { CanWrite: true } propertyInfo)
        {
            propertyInfo.SetValue(obj, value);
        }
        else
            throw new ArgumentException($"Can't set value of {member.Name} on {obj?.GetType().FullName ?? "null"}");
    }

    #region Helpers

    /// <summary>
    /// Checks whether the arguments length are equal to both members.
    /// </summary>
    /// <param name="member1">The first <see cref="MemberInfo"/>.</param>
    /// <param name="member2">The second <see cref="MemberInfo"/>.</param>
    /// <returns>True if the arguments length of the members are equal.</returns>
    internal static bool IsMemberArgumentLengthEqual(MemberInfo member1,
        MemberInfo member2)
    {
        if (member1 is MethodInfo methodInfo1 && member2 is MethodInfo methodInfo2)
        {
            return methodInfo1.GetParameters().Length == methodInfo2.GetParameters().Length;
        }

        return member1 is PropertyInfo && member2 is PropertyInfo;
    }

    #endregion
}
