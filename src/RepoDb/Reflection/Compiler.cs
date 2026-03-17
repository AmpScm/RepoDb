using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.Reflection;

/// <summary>
/// The compiler class of the library.
/// </summary>
internal sealed partial class Compiler
{
    #region SubClasses/SubStructs


    /// <summary>
    /// A class that contains both the instance of <see cref="RepoDb.ClassProperty"/> and <see cref="System.Reflection.ParameterInfo"/> objects.
    /// </summary>
    private sealed class ClassPropertyParameterInfo
    {
        private string? descriptiveContextString;

        /// <summary>
        /// Gets the instance of <see cref="RepoDb.ClassProperty"/> object in used.
        /// </summary>
        public ClassProperty? ClassProperty { get; init; }

        /// <summary>
        /// Gets the instance of <see cref="System.Reflection.ParameterInfo"/> object in used.
        /// </summary>
        public ParameterInfo? ParameterInfo { get; init; }

        /// <summary>
        /// Gets the instance of <see cref="RepoDb.ClassProperty"/> object that is mapped to the current <see cref="ParameterInfo"/>.
        /// </summary>
        public ClassProperty? ParameterInfoMappedClassProperty { get; init; }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        public Type? TargetType { get; init; }

        /// <summary>
        /// Gets the target type based on the combinations.
        /// </summary>
        /// <returns></returns>
        public Type GetTargetType() =>
            TargetType ?? ParameterInfo?.ParameterType ?? ClassProperty?.PropertyInfo?.PropertyType!;

        /// <summary>
        /// Gets the descriptive context string for error messaging.
        /// </summary>
        /// <returns></returns>
        public string GetDescriptiveContextString()
        {
            if (descriptiveContextString is not null)
            {
                return descriptiveContextString;
            }

            // Variable
            var message = $"Context :: TargetType: {GetTargetType().FullName} ";

            // ParameterInfo
            if (ParameterInfo is not null)
            {
                message = string.Concat(descriptiveContextString, $"Parameter: {ParameterInfo.Name} ({ParameterInfo.ParameterType.FullName}) ");
            }

            // ClassProperty
            if (ClassProperty?.PropertyInfo != null)
            {
                message = string.Concat(descriptiveContextString, $"PropertyInfo: {ClassProperty.PropertyInfo.Name} ({ClassProperty.PropertyInfo.PropertyType.FullName}), DeclaringType: {ClassProperty.DeclaringType.FullName} ");
            }

            // Return
            return descriptiveContextString = message.Trim();
        }

        /// <summary>
        /// Returns the string that represents this object.
        /// </summary>
        /// <returns>The presented string.</returns>
        public override string ToString() =>
            string.Concat("TargetType = ", GetTargetType()?.FullName, ", ClassProperty = ", ClassProperty?.ToString(), ", ",
                "ParameterInfo = ", ParameterInfo?.ToString(), ")", ", TargetType = ", TargetType?.ToString(), ", ");
    }

    private struct FieldDirection
    {
        public int Index { get; set; }
        public DbField DbField { get; set; }
        public ParameterDirection Direction { get; set; }
    }

    /// <summary>
    /// A class that contains both the property <see cref="MemberAssignment"/> object and the constructor argument <see cref="Expression"/> value.
    /// </summary>
    private class MemberBinding
    {
        /// <summary>
        /// Gets the instance of <see cref="ClassProperty"/> object in used.
        /// </summary>
        public ClassProperty? ClassProperty { get; init; }

        /// <summary>
        /// Gets the instance of <see cref="ParameterInfo"/> object in used.
        /// </summary>
        public ParameterInfo? ParameterInfo { get; init; }

        /// <summary>
        /// Gets the current member assignment of the defined property.
        /// </summary>
        public MemberAssignment? MemberAssignment { get; init; }

        /// <summary>
        /// Gets the corresponding constructor argument of the defined property.
        /// </summary>
        public Expression? Argument { get; init; }

        /// <summary>
        /// Returns the string that represents this object.
        /// </summary>
        /// <returns>The presented string.</returns>
        public override string ToString() =>
            ClassProperty?.ToString() ?? ParameterInfo?.ToString() ?? "";
    }

    #endregion

    #region Methods

    private static IEnumerable<FieldDirection> GetInputFieldDirections(IEnumerable<DbField>? fields)
    {
        if (fields?.Any() != true)
        {
            return [];
        }
        return fields.Select((value, index) => new FieldDirection
        {
            Index = index,
            DbField = value,
            Direction = ParameterDirection.Input
        });
    }

    private static IEnumerable<FieldDirection> GetOutputFieldDirections(IEnumerable<DbField>? fields)
    {
        if (fields?.Any() != true)
        {
            return [];
        }
        return fields.Select((value, index) => new FieldDirection
        {
            Index = index,
            DbField = value,
            Direction = ParameterDirection.Output
        });
    }

    private static MethodInfo? GetSystemConvertToTypeMethod(Type fromType,
        Type toType) =>
        StaticType.Convert.GetMethod(string.Concat("To", TypeCache.Get(toType).UnderlyingType.Name),
            [TypeCache.Get(fromType).UnderlyingType, typeof(IFormatProvider)])
        ?? StaticType.Convert.GetMethod(string.Concat("To", TypeCache.Get(toType).UnderlyingType.Name),
            [TypeCache.Get(fromType).UnderlyingType]);

    private static object? GetClassHandler(Type type) =>
        ClassHandlerCache.Get<object>(type);

    private static MethodInfo GetClassHandlerGetMethod(object handlerInstance) =>
        handlerInstance.GetType().GetMethod(nameof(IPropertyHandler<,>.Get))!;

    private static MethodInfo GetClassHandlerSetMethod(object handlerInstance) =>
        handlerInstance.GetType().GetMethod(nameof(IPropertyHandler<,>.Set))!;

    /// <summary>
    ///
    /// </summary>
    /// <param name="handlerInstance"></param>
    /// <returns></returns>
    public static Type? GetPropertyHandlerInterfaceOrHandlerType(object? handlerInstance)
    {
        if (handlerInstance is null)
            return null;

        var handlerType = handlerInstance.GetType();
        var propertyHandlerInterface = handlerType
            .GetInterfaces()
            .FirstOrDefault(interfaceType =>
                interfaceType.IsInterfacedTo(StaticType.IPropertyHandler));
        return propertyHandlerInterface ?? handlerType;
    }

    private static MethodInfo? GetPropertyHandlerGetMethod(object? handlerInstance) =>
        // In F#, the instance is not a concrete class, therefore, we need to extract it by interface
        GetPropertyHandlerInterfaceOrHandlerType(handlerInstance)?.GetMethod(nameof(IPropertyHandler<,>.Get));

    private static MethodInfo? GetPropertyHandlerGetMethodFromInterface(object handlerInstance)
    {
        var propertyHandlerInterface = handlerInstance?
            .GetType()?
            .GetInterfaces()
            .FirstOrDefault(interfaceType =>
                interfaceType.IsInterfacedTo(StaticType.IPropertyHandler));
        return propertyHandlerInterface?.GetMethod(nameof(IPropertyHandler<,>.Get));
    }

    private static MethodInfo? GetPropertyHandlerSetMethod(object? handlerInstance) =>
        GetPropertyHandlerInterfaceOrHandlerType(handlerInstance)?.GetMethod(nameof(IPropertyHandler<,>.Set));

    private static ParameterInfo? GetPropertyHandlerGetParameter(ClassPropertyParameterInfo classPropertyParameterInfo) =>
        GetPropertyHandlerGetParameter(classPropertyParameterInfo?.ClassProperty);

    private static Type? GetPropertyHandlerSetMethodReturnType(ClassProperty property,
        Type targetType) =>
        GetPropertyHandlerSetMethod(property?.GetPropertyHandler() ??
            PropertyHandlerCache.Get<object>(targetType))?.ReturnType;

    private static ParameterInfo? GetPropertyHandlerGetParameter(ClassProperty? classProperty) =>
        GetPropertyHandlerGetParameter(classProperty?.GetPropertyHandler());

    private static ParameterInfo? GetPropertyHandlerGetParameter(object? handlerInstance) =>
        GetPropertyHandlerGetParameter(GetPropertyHandlerGetMethod(handlerInstance));

    private static ParameterInfo? GetPropertyHandlerGetParameter(MethodInfo? getMethod) =>
        getMethod?.GetParameters().First();

    private static ParameterInfo? GetPropertyHandlerSetParameter(object? handlerInstance) =>
        GetPropertyHandlerSetParameter(GetPropertyHandlerSetMethod(handlerInstance));

    private static ParameterInfo? GetPropertyHandlerSetParameter(MethodInfo? setMethod) =>
        setMethod?.GetParameters().First();

    private static IEnumerable<DataReaderField> GetDataReaderFields(DbDataReader reader,
        DbFieldCollection? dbFields)
    {
        return Enumerable.Range(0, reader.FieldCount)
            .Select(reader.GetName)
            .Select((name, ordinal) => new DataReaderField
            {
                Name = name,
                Ordinal = ordinal,
                Type = reader.GetFieldType(ordinal) ?? StaticType.Object,
                DbField = dbFields?.GetByFieldName(name)
            });
    }

    private static object? GetHandlerInstance(ClassPropertyParameterInfo classPropertyParameterInfo,
        DataReaderField readerField) =>
        GetHandlerInstance(classPropertyParameterInfo.ClassProperty, readerField);

    private static object? GetHandlerInstance(ClassProperty? classProperty,
        DataReaderField readerField)
    {
        if (classProperty is null)
        {
            return null;
        }
        var value = classProperty.GetPropertyHandler();
        if (value == null && readerField?.Type != null)
        {
            value = PropertyHandlerCache
                .Get<object>(TypeCache.Get(readerField.Type).UnderlyingType);
        }
        return value;
    }

    private static PropertyInfo GetPropertyInfo<TFrom>(Expression<Func<TFrom, object?>> expression)
    {
        return (PropertyInfo)((MemberExpression)UnwrapUnary(expression.Body)).Member;

        static Expression UnwrapUnary(Expression e) => e is UnaryExpression ue ? UnwrapUnary(ue.Operand) : e;
    }

    private static MethodInfo? GetDbReaderGetValueMethod(Type targetType, Type? readerType) =>
        readerType?.GetMethod(string.Concat("Get", targetType?.Name), [StaticType.Int32])
        ?? StaticType.DbDataReader.GetMethod(string.Concat("Get", targetType?.Name));

    private static MethodInfo GetDbReaderGetValueOrDefaultMethod(DataReaderField readerField, Type readerType)
        => GetDbReaderGetValueMethod(readerField.Type, readerType) ?? GetMethodInfo<DbDataReader>((x) => x.GetValue(default));

    private static TEnum? EnumParseNull<TEnum>(string value) where TEnum : unmanaged, Enum
    {
        return Enum.TryParse<TEnum>(value, true, out var r) ? r : null;
    }

    private static TEnum? EnumParseNullDefined<TEnum>(string value) where TEnum : unmanaged, Enum
    {
        if (Enum.TryParse<TEnum>(value, true, out var r)
#if NET
            && Enum.IsDefined<TEnum>(r))
#else
            && Enum.IsDefined(typeof(TEnum), r))
#endif
        {
            return r;
        }
        else
            return null;
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static T ThrowNullableValueIsNull<T>(string expressionDescription)
    {
        throw new InvalidOperationException($"Required Nullable<{typeof(T).Name}> property {expressionDescription} evaluated to null.");
    }

    private static Expression ConvertExpressionToRemoveNullableValue(Expression expression)
    {
        var underlyingType = Nullable.GetUnderlyingType(expression.Type);
        if (underlyingType is null)
        {
            return expression; // Already non-nullable
        }

        var temp = Expression.Variable(expression.Type, "temp");
        var assign = Expression.Assign(temp, expression);

        var hasValue = Expression.Property(temp, nameof(Nullable<>.HasValue));
        var value = Expression.Property(temp, nameof(Nullable<>.Value));

        var description = Expression.Constant(
            (expression as MemberExpression)?.Member.Name ?? expression.ToString());

        var throwCall = Expression.Call(
            GetMethodInfo(() => ThrowNullableValueIsNull<int?>(""))
                .GetGenericMethodDefinition()
                .MakeGenericMethod(underlyingType),
            description);

        var conditional = Expression.Condition(hasValue, value, throwCall);

        return Expression.Block([temp], assign, conditional);
    }

    private static Expression ConvertExpressionToNullableGetValueOrDefaultExpression(Func<Expression, Expression> converter, Expression expression)
    {
        if (Nullable.GetUnderlyingType(expression.Type) != null)
        {
            var converted = converter(ConvertExpressionToRemoveNullableValue(expression));
            var nullableType = typeof(Nullable<>).MakeGenericType(converted.Type);
            return Expression.Condition(
                Expression.Property(expression, nameof(Nullable<>.HasValue)),
                Expression.Convert(converted, nullableType),
                Expression.Constant(null, nullableType)
            );
        }

        return converter(expression);
    }

    private static Expression ConvertExpressionToStringToGuid(Expression expression) =>
        Expression.New(StaticType.Guid.GetConstructor([StaticType.String])!, ConvertExpressionToRemoveNullableValue(expression));

    private static Expression ConvertExpressionToTimeSpanToDateTime(Expression expression) =>
        Expression.New(StaticType.DateTime.GetConstructor([StaticType.Int64])!,
            ConvertExpressionToRemoveNullableValue(ConvertExpressionToTimeSpanTicksExpression(expression)));

    private static MemberExpression ConvertExpressionToTimeSpanTicksExpression(Expression expression) =>
        Expression.Property(expression, GetPropertyInfo<TimeSpan>(x => x.Ticks));

    private static Expression ConvertExpressionToDateTimeToTimeSpan(Expression expression) =>
        Expression.Property(expression, GetPropertyInfo<DateTime>(d => d.TimeOfDay));
#if NET
    private static Expression ConvertExpressionToDateOnlyToDateTime(Expression expression) =>
        Expression.Call(expression, GetMethodInfo<DateOnly>((x) => x.ToDateTime(default)), Expression.Constant(default(TimeOnly)));

    private static Expression ConvertExpressionToDateTimeToDateOnly(Expression expression) =>
        Expression.Call(null, GetMethodInfo(() => DateOnly.FromDateTime(default)), expression);

    private static Expression ConvertExpressionToTimeSpanToTimeOnly(Expression expression) =>
        Expression.Call(null, GetMethodInfo(() => TimeOnly.FromTimeSpan(default)), expression);

    private static Expression ConvertExpressionToTimeOnlyToTimeSpan(Expression expression) =>
        Expression.Call(expression, GetMethodInfo<TimeOnly>((x) => x.ToTimeSpan()));
#endif

    private static MethodInfo GetMethodInfo<TFrom>(Expression<Action<TFrom>> call) => ((MethodCallExpression)call.Body).Method;

    private static MethodInfo GetMethodInfo(Expression<Action> call)
    {
        return ((MethodCallExpression)call.Body).Method;
    }

    private static bool StrictParseBoolean(string? value)
    {
        if (value is null)
        {
            return false;
        }
        else if (bool.TryParse(value, out var v))
        {
            return v;
        }
        else if (value.Length == 1)
        {
            if (value[0] == '1')
                return true;
            else if (value[0] == '0')
                return false;
        }

        throw new FormatException($"String '{value}' was not recognized as a valid Boolean");
    }

    private static decimal StrictParseDecimal(string value)
    {
        return decimal.Parse(value, CultureInfo.InvariantCulture);
    }

    private static DateTime StrictParseDateTime(string value)
    {
        return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    private static string StrictToString(DateTime value)
    {
        return value.ToString("o", CultureInfo.InvariantCulture);
    }

    private static string StrictToString(DateTimeOffset value)
    {
        return value.ToString("o", CultureInfo.InvariantCulture);
    }

    private static DateTimeOffset StrictParseDateTimeOffset(string value)
    {
        return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind); // If we have no offset assume no offset
    }

#if NET
    private static DateOnly StrictParseDateOnly(string value)
    {
        try
        {
            return DateOnly.Parse(value, CultureInfo.InvariantCulture);
        }
        catch (FormatException) when (DateTime.TryParse(value, CultureInfo.InvariantCulture, out var vv) && DateOnly.FromDateTime(vv) is { } dateOnly && dateOnly.ToDateTime(TimeOnly.MinValue) == vv)
        {
            return dateOnly; // We can't guarantee that DateOnly is only serialized in text form as dateonly :(
        }
    }

    private static string StrictToString(DateOnly value)
    {
        return value.ToString("d", CultureInfo.InvariantCulture);
    }

    private static TimeOnly StrictParseTimeOnly(string value)
    {
        try
        {
            return TimeOnly.Parse(value, CultureInfo.InvariantCulture);
        }
        catch (FormatException) when (DateTime.TryParse(value, CultureInfo.InvariantCulture, out var vv) && TimeOnly.FromDateTime(vv) is { } timeOnly)
        {
            return timeOnly;
        }
    }

    private static string StrictToString(TimeOnly value)
    {
        return value.ToString("o", CultureInfo.InvariantCulture);
    }
#endif

    private static Expression ConvertExpressionToTypeExpression(Expression expression,
        Type toType) =>
        (expression.Type != toType) ? Expression.Convert(expression, toType) : expression;

    private static Expression ConvertExpressionToEnumExpression(Expression expression,
        Type fromType,
        Type toEnumType) =>
        (fromType == StaticType.String) ?
            ConvertExpressionToEnumExpressionForString(expression, toEnumType) :
                ConvertExpressionToEnumExpressionForNonString(expression, toEnumType);

    private static BinaryExpression ConvertExpressionToEnumExpressionForString(Expression expression, Type toEnumType)
    {
        var options = GlobalConfiguration.Options.EnumHandling;
        var parseMethod = (
            options is InvalidEnumValueHandling.Cast || toEnumType.GetCustomAttribute<FlagsAttribute>() is not null
                ? GetMethodInfo<Compiler>((x) => EnumParseNull<DbType>(default!)).GetGenericMethodDefinition()
                : GetMethodInfo<Compiler>((x) => EnumParseNullDefined<DbType>(default!)).GetGenericMethodDefinition()
        ).MakeGenericMethod(toEnumType);

        var parseCall = Expression.Call(parseMethod, expression);

        Expression fallbackExpression;
        if (options == InvalidEnumValueHandling.UseDefault)
        {
            fallbackExpression = Expression.Default(toEnumType);
        }
        else
        {
            // Create static helper call for throwing, but typed as `toEnumType`
            fallbackExpression = Expression.Call(
                GetMethodInfo(() => ThrowInvalidEnumValue<DbType>(default!)).GetGenericMethodDefinition().MakeGenericMethod(toEnumType),
                expression);
        }

        return Expression.Coalesce(parseCall, fallbackExpression);
    }

    [DoesNotReturn]
    private static TEnum ThrowInvalidEnumValue<TEnum>(object? value)
        where TEnum : unmanaged, Enum
    {
        throw new ArgumentOutOfRangeException(nameof(value), value, $"Invalid value for {typeof(TEnum).Name}");
    }

    private static Expression ConvertExpressionToEnumExpressionForNonString(Expression expression,
        Type toEnumType)
    {
        if (GlobalConfiguration.Options.EnumHandling == InvalidEnumValueHandling.Cast)
        {
            return Expression.Convert(expression, toEnumType);
        }
        else
        {
            // Handle long/short to enum and/or non integer based enums
            if (expression.Type != Enum.GetUnderlyingType(toEnumType))
                expression = Expression.Convert(expression, Enum.GetUnderlyingType(toEnumType));

            return Expression.Condition(
                GetEnumIsDefinedExpression(expression, toEnumType), // Check if the value is defined
                Expression.Convert(expression, toEnumType), // Cast to enum
                GlobalConfiguration.Options.EnumHandling switch
                {
                    InvalidEnumValueHandling.UseDefault => Expression.Default(toEnumType),
                    InvalidEnumValueHandling.ThrowError => Expression.Throw(Expression.New(typeof(InvalidEnumArgumentException).GetConstructor([StaticType.String, StaticType.Int32, StaticType.Type])!,
                                                                [Expression.Constant("value"), Expression.Convert(expression, StaticType.Int32), Expression.Constant(toEnumType)]),
                        toEnumType
                    ),
                    _ => throw new InvalidEnumArgumentException("EnumHandling set to invalid value")
                }); // Default value for undefined
        }
    }

    private static Expression ConvertEnumExpressionToTypeExpression(Expression expression,
        Type toType)
    {
        var underlyingType = TypeCache.Get(toType).UnderlyingType;
        if (underlyingType == StaticType.String || underlyingType == StaticType.Boolean)
        {
            return ConvertEnumExpressionToTypeExpressionForString(expression);
        }
        else
        {
            return ConvertEnumExpressionToTypeExpressionForNonString(expression, toType);
        }
    }

    private static Expression ConvertEnumExpressionToTypeExpressionForString(Expression expression)
    {
        var method = GetMethodInfo(() => Convert.ToString((object?)null, CultureInfo.InvariantCulture));

        // Variables
        Expression? isNullExpression = null;
        Expression? trueExpression = null;
        Expression falseExpression;

        // Ensure (Ref/Nullable)
        if (TypeCache.Get(expression.Type).IsNullable)
        {
            // Check
            isNullExpression = Expression.Equal(Expression.Constant(null), expression);

            // True
            trueExpression = Expression.Convert(Expression.Constant(null), StaticType.String);
        }

        // False
        var methodCallExpression = Expression.Call(method, ConvertExpressionToTypeExpression(expression, StaticType.Object), Expression.Constant(CultureInfo.InvariantCulture));
        falseExpression = ConvertExpressionToTypeExpression(methodCallExpression, StaticType.String);

        // Call and return
        return isNullExpression == null ? falseExpression :
            Expression.Condition(isNullExpression, trueExpression!, falseExpression);
    }

    private static Expression ConvertEnumExpressionToTypeExpressionForNonString(Expression expression,
        Type toType)
    {
        Expression? isNullExpression = null;
        Expression? trueExpression = null;
        var falseExpression = expression;

        // Ensure (Ref/Nullable)
        var cachedType = TypeCache.Get(expression.Type);
        if (cachedType.IsNullable)
        {
            isNullExpression = Expression.Equal(Expression.Constant(null), expression);
            trueExpression = GetNullableTypeExpression(toType);
        }

        // Casting
        if (TypeCache.Get(toType).UnderlyingType is { } tt && cachedType.UnderlyingType != tt)
        {
            if (tt != StaticType.Decimal)
            {
                falseExpression = ConvertExpressionToTypeExpression(expression, tt);
            }
            else
            {
                // Oracle loves decimal
                falseExpression =
                    ConvertExpressionToTypeExpression(
                        ConvertExpressionToTypeExpression(expression, Enum.GetUnderlyingType(cachedType.UnderlyingType)),
                        tt);
            }
        }

        // Nullable
        if (cachedType.IsNullable)
        {
            falseExpression = ConvertExpressionToNullableExpression(falseExpression, toType);
        }

        // Return
        return isNullExpression == null ? falseExpression :
            Expression.Condition(isNullExpression, trueExpression!, falseExpression);
    }

    private static ConditionalExpression ConvertExpressionToDbNullExpression(Expression expression)
    {
        var valueIsNullExpression = Expression.Equal(expression, Expression.Constant(null));
        var dbNullValueExpresion = ConvertExpressionToTypeExpression(Expression.Constant(DBNull.Value), StaticType.Object);
        return Expression.Condition(valueIsNullExpression, dbNullValueExpresion, expression);
    }

    private static Expression ConvertExpressionToNullableExpression(Expression expression,
        Type targetNullableType)
    {
        if (!expression.Type.IsValueType)
        {
            return expression;
        }

        var underlyingType = Nullable.GetUnderlyingType(expression.Type);
        targetNullableType = TypeCache.Get(targetNullableType).UnderlyingType;

        if (targetNullableType.IsValueType && (underlyingType == null || underlyingType != targetNullableType))
        {
            var nullableType = StaticType.Nullable.MakeGenericType(targetNullableType);
            var constructor = nullableType.GetConstructor([targetNullableType])!;
            expression = TypeCache.Get(expression.Type).IsNullable ? expression :
                Expression.New(constructor, ConvertExpressionToTypeExpression(expression, targetNullableType));
        }

        return expression;
    }

    private static Expression ConvertExpressionWithAutomaticConversion(Expression expression,
        Type toType)
    {
        var fromType = expression.Type;
        var underlyingFromType = TypeCache.Get(expression.Type).UnderlyingType;
        var underlyingToType = TypeCache.Get(toType).UnderlyingType;

        if (fromType == toType)
        {
            // NO-OP
        }
        else if (underlyingFromType == underlyingToType)
        {
            // Just nullable handling
            if (underlyingFromType == fromType)
            {
                return Expression.Convert(expression, toType); // Make nullable
            }
            else
            {
                return ConvertExpressionToRemoveNullableValue(expression);
            }
        }
        else if (underlyingFromType.IsBinaryIntFloatOrDecimal() && underlyingToType.IsBinaryIntFloatOrDecimal())
        {
            // This case used to be handled as final fallback by using Convert.To[type](fromType), but had side effects like bankers rounding
            // Lets do this clean and performant
            var result = Expression.ConvertChecked(ConvertExpressionToRemoveNullableValue(expression), underlyingToType);

            if (toType != underlyingToType && underlyingFromType != expression.Type)
            {
                expression = Expression.Condition(Expression.Property(expression, nameof(Nullable<>.HasValue)), Expression.Convert(result, toType), Expression.Constant(null, toType));
            }
            else
                expression = result;
        }
        // Guid to String
        else if (underlyingFromType == StaticType.Guid && underlyingToType == StaticType.String)
        {
            var result = Expression.Call(ConvertExpressionToRemoveNullableValue(expression), GetMethodInfo<Guid>(x => x.ToString()));

            if (underlyingFromType != expression.Type)
            {
                // Handle nullability
                expression = Expression.Condition(
                    Expression.Property(expression, nameof(Nullable<>.HasValue)),
                    result,
                    Expression.Constant(null, StaticType.String)
                    );
            }
            else
                expression = result;
        }
        else if (underlyingFromType == StaticType.Guid && underlyingToType == StaticType.ByteArray)
        {
            var result = Expression.Call(ConvertExpressionToRemoveNullableValue(expression), GetMethodInfo<Guid>(x => x.ToByteArray()));

            if (underlyingFromType != expression.Type)
            {
                // Handle nullability
                expression = Expression.Condition(
                    Expression.Property(expression, nameof(Nullable<>.HasValue)),
                    result,
                    Expression.Constant(null, StaticType.ByteArray)
                    );
            }
            else
                expression = result;
        }
        // String to Guid
        else if (underlyingFromType == StaticType.String && underlyingToType == StaticType.Guid)
        {
            expression = ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToStringToGuid, expression);
        }
        else if (underlyingFromType == StaticType.TimeSpan && underlyingToType == StaticType.DateTime)
        {
            expression = ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToTimeSpanToDateTime, expression);
        }
        else if (underlyingFromType == StaticType.DateTime && underlyingToType == StaticType.TimeSpan)
        {
            expression = ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToDateTimeToTimeSpan, expression);
        }
#if NET
        else if (underlyingFromType == StaticType.DateTime && underlyingToType == StaticType.DateOnly)
        {
            expression = ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToDateTimeToDateOnly, expression);
        }
        else if (underlyingFromType == StaticType.DateOnly && underlyingToType == StaticType.DateTime)
        {
            expression = ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToDateOnlyToDateTime, expression);
        }
        else if (underlyingFromType == StaticType.TimeSpan && underlyingToType == StaticType.TimeOnly)
        {
            expression = ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToTimeSpanToTimeOnly, expression);
        }
        else if (underlyingFromType == StaticType.TimeOnly && underlyingToType == StaticType.TimeSpan)
        {
            expression = ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToTimeOnlyToTimeSpan, expression);
        }
#endif
        else if (underlyingToType == StaticType.SqlVariant || underlyingToType == typeof(object))
        {
            // NO-OP
        }
        else if (underlyingToType.IsAssignableFrom(fromType))
        {
            return Expression.Convert(expression, underlyingToType);
        }
        else
        {
            return ConvertExpressionToSystemConvertExpression();
        }

        // Return
        return expression;

        Expression ConvertExpressionToSystemConvertExpression()
        {
            var result = ConvertExpressionToRemoveNullableValue(expression);

            // Convert.To<Type>()
            if (ProviderSpecificTransforms.TryGetValue((fromType, toType), out var transform)
                && transform?.Invoke(expression) is { } r)
            {
                result = r;
            }
            else if (underlyingFromType.IsEnum)
            {
                if (underlyingToType == StaticType.String)
                {
                    result = Expression.Call(result, nameof(ToString), []);
                }
                else if (underlyingToType.IsBinaryInteger())
                {
                    result = Expression.Convert(result, Enum.GetUnderlyingType(underlyingFromType));

                    if (result.Type != underlyingToType)
                    {
                        result = Expression.Convert(result, underlyingToType);
                    }
                }
                else if (underlyingToType == StaticType.Decimal)
                {
                    // Oracle loves decimal
                    result = Expression.Convert(result, Enum.GetUnderlyingType(underlyingFromType));

                    if (result.Type != underlyingToType)
                    {
                        result = Expression.Convert(result, underlyingToType);
                    }
                }
                else
                {
                    return result; // Will fail
                }
            }
            else if (toType == StaticType.String && underlyingFromType == StaticType.DateTime)
            {
                var expr = (fromType == underlyingFromType) ? expression : Expression.Convert(expression, underlyingFromType);
                result = Expression.Call(GetMethodInfo(() => StrictToString(DateTime.MinValue)), expr);
            }
            else if (toType == StaticType.String && underlyingFromType == StaticType.DateTimeOffset)
            {
                var expr = (fromType == underlyingFromType) ? expression : Expression.Convert(expression, underlyingFromType);
                result = Expression.Call(GetMethodInfo(() => StrictToString(DateTimeOffset.MinValue)), expr);
            }
            else if (toType == StaticType.String && fromType.IsJsonNode())
            {
                result = Expression.Call(result, GetMethodInfo<JsonNode>(x => x.ToJsonString(null)), Expression.Constant(Converter.JsonSerializerOptions));
            }
            else if (fromType == typeof(string) && toType.IsJsonNode())
            {
                result = Expression.Call(GetMethodInfo(() => JsonNode.Parse("", nodeOptions: null, documentOptions: default)), [result, Expression.Default(typeof(JsonNodeOptions?)), Expression.Default(typeof(JsonDocumentOptions))]);

                if (result.Type != toType)
                {
                    result = Expression.Convert(result, toType);
                }
            }
#if NET
            else if (toType == StaticType.String && underlyingFromType == StaticType.DateOnly)
            {
                var expr = (fromType == underlyingFromType) ? expression : Expression.Convert(expression, underlyingFromType);

                result = Expression.Call(GetMethodInfo(() => StrictToString(DateOnly.MinValue)), expr);
            }
            else if (toType == StaticType.String && underlyingFromType == StaticType.TimeOnly)
            {
                var expr = (fromType == underlyingFromType) ? expression : Expression.Convert(expression, underlyingFromType);

                result = Expression.Call(GetMethodInfo(() => StrictToString(TimeOnly.MinValue)), expr);
            }
#endif
            else if (underlyingToType == StaticType.DateTime && underlyingFromType == StaticType.DateTimeOffset)
            {
                result = Expression.Property(result, nameof(DateTimeOffset.DateTime));
            }
            else if (underlyingToType == StaticType.DateTimeOffset && underlyingFromType == StaticType.DateTime)
            {
                result = Expression.New(typeof(DateTimeOffset).GetConstructor([typeof(DateTime)])!, [result]);
            }
            else if (underlyingToType == StaticType.Boolean && fromType == StaticType.String)
            {
                result = Expression.Call(GetMethodInfo(() => StrictParseBoolean(null)), expression);
            }
            else if (fromType == StaticType.String && underlyingToType == StaticType.Decimal)
            {
                result = Expression.Call(GetMethodInfo(() => StrictParseDecimal(null!)), expression);
            }
            else if (fromType == StaticType.String && underlyingToType == StaticType.DateTime)
            {
                result = Expression.Call(GetMethodInfo(() => StrictParseDateTime(null!)), expression);
            }
            else if (fromType == StaticType.String && underlyingToType == StaticType.DateTimeOffset)
            {
                result = Expression.Call(GetMethodInfo(() => StrictParseDateTimeOffset(null!)), expression);
            }
#if NET
            else if (fromType == StaticType.String && underlyingToType == typeof(DateOnly))
            {
                var expr = (fromType == underlyingFromType) ? expression : Expression.Convert(expression, underlyingFromType);
                result = Expression.Call(GetMethodInfo(() => StrictParseDateOnly(null!)), expr);
            }
            else if (fromType == StaticType.String && underlyingToType == typeof(TimeOnly))
            {
                var expr = (fromType == underlyingFromType) ? expression : Expression.Convert(expression, underlyingFromType);
                result = Expression.Call(GetMethodInfo(() => StrictParseTimeOnly(null!)), expr);
            }
#endif
            else if (fromType == typeof(string) && underlyingToType.ImplementsIParsable())
            {
                var parseMethod = underlyingToType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, [typeof(string), typeof(IFormatProvider)])!;
                result = Expression.Call(parseMethod, expression, Expression.Constant(CultureInfo.InvariantCulture));
            }
            else if (toType == typeof(string) && typeof(IFormattable).IsAssignableFrom(underlyingFromType))
            {
                var toStringMethod = GetMethodInfo<IFormattable>(x => x.ToString(null, null));

                var expr = (fromType == underlyingFromType) ? expression : Expression.Convert(expression, underlyingFromType);
                expr = Expression.Convert(expr, typeof(IFormattable));

                result = Expression.Call(expr, toStringMethod, Expression.Constant(null, typeof(string)), Expression.Constant(CultureInfo.InvariantCulture, typeof(IFormatProvider)));
            }
            else if (toType.IsJsonNode() && typeof(IDbJsonValue).IsAssignableFrom(underlyingFromType))
            {
                var expr = (fromType == underlyingFromType) ? expression : result;

                result = Expression.Property(expr, GetPropertyInfo<IDbJsonValue>(x => x.JsonNode));

                if (result.Type != toType)
                    result = Expression.Convert(result, toType);
            }
            else if (underlyingToType == typeof(bool) && underlyingFromType.IsBinaryIntFloatOrDecimal())
            {
                result = Expression.NotEqual(result, Expression.Convert(Expression.Constant(0), underlyingFromType));
            }
            else if (underlyingFromType == typeof(bool) && underlyingToType.IsBinaryIntFloatOrDecimal())
            {
                result = Expression.Condition(result, Expression.Convert(Expression.Constant(1), underlyingToType), Expression.Convert(Expression.Constant(0), underlyingToType));
            }
            else if (toType == typeof(string)
                && underlyingFromType.GetMethod(nameof(object.ToString), [typeof(IFormatProvider)]) is { } numberFormatMethod)
            {
                result = Expression.Call(result, numberFormatMethod, [Expression.Constant(CultureInfo.InvariantCulture, typeof(IFormatProvider))]);
            }
#if NET
            else if (toType == typeof(Half) && (underlyingFromType.IsBinaryIntFloatOrDecimal() || underlyingFromType == StaticType.Object))
            {
                // System.Converter doesn't support half, nor does 99% of DotNet. Just fall through to float/single support and convert from there

                expression = ConvertExpressionWithAutomaticConversion(expression, typeof(float)); // Convert to float first

                result = Expression.ConvertChecked(expression, typeof(Half));
            }
#endif
            else if (GetSystemConvertToTypeMethod(underlyingFromType, underlyingToType) is { } methodInfo)
            {
                var param = methodInfo.GetParameters();
                if (param.Length == 1)
                    result = Expression.Call(methodInfo, Expression.Convert(result, param[0].ParameterType));
                else
                    result = Expression.Call(methodInfo, Expression.Convert(result, param[0].ParameterType), Expression.Constant(CultureInfo.InvariantCulture, typeof(IFormatProvider)));

                // Perhaps we sometimes only have to unbox. Lets inline that support
                if (fromType == typeof(object))
                {
                    result = Expression.Condition(
                        Expression.TypeIs(expression, underlyingToType),
                        Expression.Convert(expression, underlyingToType),
                        result);
                }
            }
            else
            {
                var systemChangeType = GetMethodInfo(() => WrapConvertChangeType(default!, (Type)null!, (Type)null!));

                // PostgreSql somehow does a lot of System.Object -> System.Array. Check this cheap return first to avoid a lot of reflection
                result = Expression.Condition(
                    Expression.TypeIs(expression, underlyingToType),
                    // This case happens in PostgreSql bulktests
                    Expression.Convert(expression, typeof(object)),
                    // And this currently doesn't happen in tests, but hard failing because IConverable doesn't support types causes quite some issues
                    Expression.Call(systemChangeType,
                    [
                        ConvertExpressionToTypeExpression(result, StaticType.Object),
                        Expression.Constant(fromType, typeof(Type)),
                        Expression.Constant(underlyingToType, typeof(Type)),
                    ])
                );

            }

            // Do we need manual NULL handling?
            if ((!underlyingToType.IsValueType || underlyingToType != toType)
                && (!underlyingFromType.IsValueType || underlyingFromType != fromType))
            {
                Expression condition;
                if (underlyingFromType != fromType)
                {
                    // E.g. Nullable<System.Int32> -> string
                    condition = Expression.Property(expression, nameof(Nullable<>.HasValue));
                }
                else
                {
                    // E.g. String -> Nullable<System.Int32>
                    condition = Expression.NotEqual(expression, Expression.Constant(null, expression.Type));
                }

                return Expression.Condition(
                    condition,
                    (result.Type != toType) ? Expression.Convert(result, toType) : result,
                    Expression.Constant(null, toType));
            }

            // Return
            return result;
        }
    }

    private static object? WrapConvertChangeType(object? value, Type fromType, Type conversionType)
    {
        var actualType = value?.GetType();

        try
        {
            if (actualType is { } && Compiler.TryConvertViaProvider(value, conversionType, actualType, out var newValue))
            {
                return newValue;
            }

            if (value is IConvertible) // Convert.ChangeType only supports system types that support IConvertable
                return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
            else
                return value; // We can't convert it... fall through to the db provider, etc.
        }
        catch (Exception ex)
        {
            throw MakeException(actualType, conversionType, fromType, ex);
        }

        static InvalidOperationException MakeException(Type? valueType, Type conversionType, Type fromType, Exception innerException)
            => new InvalidOperationException($"No declared converter found to convert value of type '{valueType?.FullName ?? "null"}' (declared as'{fromType.FullName}') to type '{conversionType.FullName}', even via final reflection fallback.", innerException);
    }

    internal static bool TryConvertViaProvider(object value, Type toType, Type valueType, out object? newValue)
    {
        // Perhaps the DB provider can do something smart here...
        if (ProviderSpecificTransforms.TryGetValue((valueType, toType), out var metaTransform)
            && Expression.Parameter(valueType) is { } param
            && metaTransform(param) is { } transform)
        {
            newValue = Expression.Lambda(transform, param).Compile(true).DynamicInvoke(value);
            return true;
        }
        else
        {
            newValue = null;
            return false;
        }
    }

    private static Expression ConvertExpressionToPropertyHandlerGetExpression(Expression expression,
        Expression readerExpression,
        object? handlerInstance,
        ClassPropertyParameterInfo classPropertyParameterInfo)
    {
        // Return if null
        if (handlerInstance is null)
        {
            return expression;
        }

        // Variables Needed
        var getMethod = GetPropertyHandlerGetMethod(handlerInstance)!;
        var getParameter = GetPropertyHandlerGetParameter(getMethod)!;

        // Call the PropertyHandler.Get
        expression = Expression.Call(Expression.Constant(handlerInstance), getMethod,
        [
            ConvertExpressionToTypeExpression(expression, getParameter.ParameterType),
            CreatePropertyHandlerGetOptionsExpression(readerExpression, classPropertyParameterInfo.ClassProperty)
        ]);

        // Convert to the return type
        return ConvertExpressionToTypeExpression(expression, getMethod.ReturnType);
    }

    private static Expression ConvertExpressionToClassHandlerGetExpression<TResult>(Expression entityExpression,
        ParameterExpression readerParameterExpression)
    {
        var typeOfResult = typeof(TResult);

        // Check the handler
        var handlerInstance = GetClassHandler(typeOfResult);
        if (handlerInstance is null)
        {
            return entityExpression;
        }

        // Validate
        var handlerType = handlerInstance.GetType();
        if (!handlerType.IsClassHandlerValidForModel(typeOfResult))
        {
            throw new InvalidTypeException($"The class handler '{handlerType.FullName}' cannot be used for the type '{typeOfResult.FullName}'.");
        }

        // Call the ClassHandler.Get method
        var getMethod = GetClassHandlerGetMethod(handlerInstance);
        return Expression.Call(Expression.Constant(handlerInstance),
            getMethod,
            entityExpression,
            CreateClassHandlerGetOptionsExpression(readerParameterExpression));
    }

    private static Expression ConvertExpressionToPropertyHandlerSetExpression(Expression expression,
        Expression? parameterExpression,
        ClassProperty? classProperty,
        Type? targetType) =>
        ConvertExpressionToPropertyHandlerSetExpressionTuple(expression, parameterExpression, classProperty, targetType).convertedExpression;

    private static (Expression convertedExpression, Type? handlerSetReturnType) ConvertExpressionToPropertyHandlerSetExpressionTuple(Expression expression,
        Expression? parameterExpression,
        ClassProperty? classProperty,
        Type? targetType)
    {
        var handlerInstance = classProperty?.GetPropertyHandler() ??
            (targetType is { } tt ? PropertyHandlerCache.Get<object>(tt) : null);

        // Check
        if (handlerInstance is null)
        {
            return (expression, null);
        }

        // Variables
        var setMethod = GetPropertyHandlerSetMethod(handlerInstance)!;
        var setParameter = GetPropertyHandlerSetParameter(setMethod)!;

        // Nullable
        expression = ConvertExpressionToNullableExpression(expression,
            TypeCache.Get(setParameter.ParameterType).UnderlyingType);

        // Call
        var valueExpression = ConvertExpressionToTypeExpression(expression, setParameter.ParameterType);
        expression = Expression.Call(Expression.Constant(handlerInstance),
            setMethod,
            [
                valueExpression,
                CreatePropertyHandlerSetOptionsExpression(parameterExpression,classProperty)
            ]);

        // Align
        return (ConvertExpressionToTypeExpression(expression, setMethod.ReturnType), setMethod.ReturnType);
    }

    private static Expression ConvertExpressionToClassHandlerSetExpression(Expression commandExpression,
        Type resultType,
        Expression entityOrEntitiesExpression)
    {
        if (GetClassHandler(resultType) is not { } handlerInstance)
            return entityOrEntitiesExpression;

        // Validate
        var handlerType = handlerInstance.GetType();
        if (!handlerType.IsClassHandlerValidForModel(resultType))
        {
            throw new InvalidTypeException($"The class handler '{handlerType.FullName}' cannot be used for type '{resultType.FullName}'.");
        }

        // Call the IClassHandler.Set method
        //var typeOfListEntity = typeof(IList<>).MakeGenericType(StaticType.Object);
        //var type = typeOfListEntity.IsAssignableFrom(entityOrEntitiesExpression.Type) ?
        //    typeOfListEntity : resultType;
        var setMethod = GetClassHandlerSetMethod(handlerInstance);
        entityOrEntitiesExpression = Expression.Call(Expression.Constant(handlerInstance),
            setMethod,
            ConvertExpressionToTypeExpression(entityOrEntitiesExpression, resultType),
            CreateClassHandlerSetOptionsExpression(commandExpression));

        // Return the block
        return entityOrEntitiesExpression;
    }

    #endregion

    #region Common

    private static MethodCallExpression GetEnumIsDefinedExpression(Expression expression,
        Type enumType)
    {
        return Expression.Call(
            GetMethodInfo<Enum>((x) => Enum.IsDefined(default!, default!)),
            [
                Expression.Constant(enumType),
                ConvertExpressionToTypeExpression(expression, StaticType.Object)
            ]);
    }

    private static Expression GetClassPropertyParameterInfoValueExpression(
        ParameterExpression readerParameterExpression,
        ClassPropertyParameterInfo classPropertyParameterInfo,
        DataReaderField readerField,
        Type readerType)
    {
        // False expression
        var falseExpression = GetClassPropertyParameterInfoIsDbNullFalseValueExpression(readerParameterExpression,
            classPropertyParameterInfo, readerField, readerType);

        // Skip if possible
        if (readerField.DbField?.IsNullable == false)
        {
            return falseExpression;
        }

        // IsDbNull Check
        var isDbNullExpression = Expression.Call(readerParameterExpression,
            StaticType.DbDataReader.GetMethod(nameof(DbDataReader.IsDBNull))!, Expression.Constant(readerField.Ordinal));

        // True Expression
        var trueExpression = GetClassPropertyParameterInfoIsDbNullTrueValueExpression(readerParameterExpression,
            classPropertyParameterInfo, readerField);

        // Set the value
        return Expression.Condition(isDbNullExpression, trueExpression, falseExpression);
    }

    private static Expression GetClassPropertyParameterInfoIsDbNullTrueValueExpression(Expression readerExpression,
        ClassPropertyParameterInfo classPropertyParameterInfo,
        DataReaderField readerField)
    {
        var parameterType = GetPropertyHandlerGetParameter(classPropertyParameterInfo)?.ParameterType;
        var classPropertyParameterInfoType = classPropertyParameterInfo.GetTargetType();

        // get handler on class property or type level. for detect default value type and convert
        var handlerInstance = GetHandlerInstance(classPropertyParameterInfo, readerField) ?? PropertyHandlerCache.Get<object>(classPropertyParameterInfo.GetTargetType()!);

        // default value expression
        var valueType = handlerInstance == null ?
            parameterType ?? classPropertyParameterInfoType :
            GetPropertyHandlerGetParameter(GetPropertyHandlerGetMethod(handlerInstance)!)!.ParameterType;
        Expression valueExpression = Expression.Default(valueType);

        // Property Handler
        try
        {
            valueExpression = ConvertExpressionToPropertyHandlerGetExpression(valueExpression, readerExpression, handlerInstance!, classPropertyParameterInfo);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.TrueExpression: Failed to convert the value expression for property handler '{handlerInstance?.GetType()}'. " +
                $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
        }

        // Align the type
        try
        {
            valueExpression = ConvertExpressionToTypeExpression(valueExpression, classPropertyParameterInfoType);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.TrueExpression: Failed to convert the value expression into its destination .NET CLR Type '{classPropertyParameterInfoType.FullName}'. " +
                $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
        }

        // Return
        return valueExpression;
    }

    private static Expression GetClassPropertyParameterInfoIsDbNullFalseValueExpression(ParameterExpression readerParameterExpression,
        ClassPropertyParameterInfo classPropertyParameterInfo,
        DataReaderField readerField,
        Type readerType)
    {
        var parameterType = GetPropertyHandlerGetParameter(classPropertyParameterInfo)?.ParameterType;
        var classPropertyParameterInfoType = classPropertyParameterInfo.GetTargetType();
        var targetType = parameterType ?? classPropertyParameterInfoType;
        var readerGetValueMethod = GetDbReaderGetValueOrDefaultMethod(readerField, readerType);
        var valueExpression = (Expression)GetDbReaderGetValueExpression(readerParameterExpression,
            readerGetValueMethod, readerField.Ordinal);
        var targetTypeUnderlyingType = TypeCache.Get(targetType).UnderlyingType;

        // get handler on class property or type level
        var handlerInstance = GetHandlerInstance(classPropertyParameterInfo, readerField) ?? PropertyHandlerCache.Get<object>(classPropertyParameterInfo.GetTargetType());

        // Enumerations
        if (targetTypeUnderlyingType.IsEnum)
        {
            // If it has a PropertyHandler and the parameter type is matching, then, skip the auto conversion.
            var autoConvertEnum = true;
            if (handlerInstance is not null)
            {
                var getParameter = GetPropertyHandlerGetParameter(GetPropertyHandlerGetMethod(handlerInstance))!;
                autoConvertEnum = !(TypeCache.Get(getParameter.ParameterType).UnderlyingType == readerField.Type);
            }
            if (autoConvertEnum)
            {
                try
                {
                    valueExpression = ConvertExpressionToEnumExpression(valueExpression, readerField.Type, targetTypeUnderlyingType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to convert the value expression into enum type '{targetType.GetUnderlyingType()}'. " +
                        $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
                }

            }
        }
        else
        {
            // Auto-conversion
            try
            {
                valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, targetType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.FalseExpression: Failed to automatically convert the value expression. " +
                    $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
            }
        }

        // Property Handler
        try
        {
            valueExpression = ConvertExpressionToPropertyHandlerGetExpression(
                valueExpression, readerParameterExpression, handlerInstance, classPropertyParameterInfo);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.FalseExpression: Failed to convert the value expression for property handler '{handlerInstance?.GetType()}'. " +
                $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
        }

        // Align the type
        try
        {
            valueExpression = ConvertExpressionToTypeExpression(valueExpression, classPropertyParameterInfoType);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.FalseExpression: Failed to convert the value expression into its destination .NET CLR Type '{classPropertyParameterInfoType.FullName}'. " +
                $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
        }

        // Return
        return valueExpression;
    }

    private static Expression GetNullableTypeExpression(Type targetType) =>
        targetType.IsValueType
        ? Expression.New(StaticType.Nullable.MakeGenericType(TypeCache.Get(targetType).UnderlyingType))
        : Expression.Constant(null, targetType);

    private static List<ClassPropertyParameterInfo> GetClassPropertyParameterInfos<TResult>(IEnumerable<string> readerFieldsName)
    {
        var typeOfResult = typeof(TResult);
        var list = new List<ClassPropertyParameterInfo>();

        // Parameter information
        var constructorInfo = typeOfResult.GetConstructorWithMostArguments();
        var parameterInfos = constructorInfo?.GetParameters().AsList();

        // Class properties
        var classProperties = PropertyCache
            .Get(typeOfResult)
            .Where(property => readerFieldsName.Any(field => string.Equals(field, property.FieldName, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        // ParameterInfos
        foreach (var parameterInfo in parameterInfos ?? [])
        {
            if (classProperties.GetByPropertyName(parameterInfo.Name) is { } classProperty)
            {
                list.Add(new ClassPropertyParameterInfo
                {
                    ClassProperty = classProperty, //classProperty.PropertyInfo.CanWrite ? classProperty : null,
                    ParameterInfo = parameterInfo,
                    ParameterInfoMappedClassProperty = classProperty
                });
            }
        }

        // ClassProperties
        foreach (var property in classProperties.Where(property => property.PropertyInfo.CanWrite))
        {
            var listItem = list.FirstOrDefault(item => item.ClassProperty == property);
            if (listItem is not null)
                continue;

            list.Add(new ClassPropertyParameterInfo { ClassProperty = property });
        }

        // Return the list
        return list;
    }

    /// <summary>
    /// Returns the list of the bindings for the entity.
    /// </summary>
    /// <typeparam name="TResult">The target entity type.</typeparam>
    /// <param name="readerParameterExpression">The data reader parameter.</param>
    /// <param name="readerFields">The list of fields to be bound from the data reader.</param>
    /// <param name="readerType"></param>
    /// <returns>The enumerable list of <see cref="MemberBinding"/> objects.</returns>
    private static List<MemberBinding> GetMemberBindingsForDataEntity<TResult>(ParameterExpression readerParameterExpression,
        IEnumerable<DataReaderField> readerFields,
        Type readerType)
    {
        // Variables needed
        var readerFieldsName = readerFields.Select(f => f.Name).AsList();
        var classProperties = GetClassPropertyParameterInfos<TResult>(readerFieldsName);

        // Check the presence
        if (classProperties?.Count is not > 0)
        {
            return [];
        }

        // Variables needed
        var memberBindings = new List<MemberBinding>();

        // Iterate each properties
        foreach (var p in classProperties)
        {
            var mappedName = p.ParameterInfoMappedClassProperty?.FieldName ??
                p.ParameterInfo?.Name ??
                p.ClassProperty?.FieldName;

            // Skip if not found
            if (mappedName is null || !readerFieldsName.Contains(mappedName, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            // Get the value expression
            var readerField = readerFields.First(f => string.Equals(f.Name, mappedName, StringComparison.OrdinalIgnoreCase));
            var expression = GetClassPropertyParameterInfoValueExpression(readerParameterExpression,
                p, readerField, readerType);

            try
            {
                // Member values
                var memberAssignment = p.ClassProperty?.PropertyInfo?.CanWrite == true ?
                    Expression.Bind(p.ClassProperty.PropertyInfo, expression) : null;
                var argument = p.ParameterInfo != null ? expression : null;

                // Add the bindings
                memberBindings.Add(new MemberBinding
                {
                    ClassProperty = p.ClassProperty,
                    ParameterInfo = p?.ParameterInfo,
                    MemberAssignment = memberAssignment,
                    Argument = argument
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.MemberBinding: Failed to bind the value expression into a property/ctor-argument. " +
                    $"{p.GetDescriptiveContextString()}", ex);
            }
        }

        // Return the value
        return memberBindings;
    }

    private static MethodCallExpression GetDbNullExpression(ParameterExpression readerParameterExpression,
        int ordinal) =>
        GetDbNullExpression(readerParameterExpression, Expression.Constant(ordinal));

    private static MethodCallExpression GetDbNullExpression(ParameterExpression readerParameterExpression,
        ConstantExpression ordinalExpression) =>
        Expression.Call(readerParameterExpression, GetMethodInfo<DbDataReader>(x => x.IsDBNull(0)), ordinalExpression);

    private static MethodCallExpression GetDbReaderGetValueExpression(ParameterExpression readerParameterExpression,
        MethodInfo readerGetValueMethod,
        int ordinal) =>
        GetDbReaderGetValueExpression(readerParameterExpression, readerGetValueMethod, Expression.Constant(ordinal));

    private static MethodCallExpression GetDbReaderGetValueExpression(ParameterExpression readerParameterExpression,
        MethodInfo readerGetValueMethod,
        ConstantExpression ordinalExpression) =>
        Expression.Call(
            readerGetValueMethod.DeclaringType == StaticType.DbDataReader
            ? readerParameterExpression
            : Expression.Convert(readerParameterExpression, readerGetValueMethod.DeclaringType!), readerGetValueMethod, ordinalExpression);

    /// <summary>
    /// Returns the list of the bindings for the object.
    /// </summary>
    /// <param name="readerParameterExpression">The data reader parameter.</param>
    /// <param name="readerFields">The list of fields to be bound from the data reader.</param>
    /// <param name="readerType"></param>
    /// <returns>The enumerable list of child elements initializations.</returns>
    private static List<ElementInit> GetMemberBindingsForDictionary(ParameterExpression readerParameterExpression,
        List<DataReaderField> readerFields,
        Type readerType)
    {
        // Initialize variables
        var elementInits = new List<ElementInit>();
        var addMethod = StaticType.IDictionaryStringObject.GetMethod(nameof(IDictionary<,>.Add), [StaticType.String, StaticType.Object])!;

        // Iterate each properties
        for (var ordinal = 0; ordinal < readerFields.Count; ordinal++)
        {
            var readerField = readerFields[ordinal];
            var readerGetValueMethod = GetDbReaderGetValueOrDefaultMethod(readerField, readerType);
            var expression = (Expression)GetDbReaderGetValueExpression(readerParameterExpression, readerGetValueMethod, ordinal);

            // Check for nullables
            if (readerField.DbField == null || readerField.DbField.IsNullable)
            {
                var isDbNullExpression = GetDbNullExpression(readerParameterExpression, ordinal);
                var toType = (readerField.Type?.IsValueType != true) ? (readerField.Type ?? StaticType.Object) : StaticType.Object;
                expression = Expression.Condition(isDbNullExpression, Expression.Default(toType),
                    ConvertExpressionToTypeExpression(expression, toType));
            }

            // Add to the bindings
            var values = new Expression[]
            {
                Expression.Constant(readerField.Name),
                ConvertExpressionToTypeExpression(expression, StaticType.Object)
            };
            elementInits.Add(Expression.ElementInit(addMethod, values));
        }

        // Return the result
        return elementInits;
    }

    private static Expression GetEntityInstancePropertyValueExpression(Expression entityInstanceExpression,
        ClassProperty classProperty,
        DbField dbField)
    {
        var expression = (Expression)Expression.Property(entityInstanceExpression, classProperty.PropertyInfo);

        // Target type
        var handlerInstance = classProperty.GetPropertyHandler() ?? PropertyHandlerCache.Get<object>(TypeCache.Get(dbField.Type).UnderlyingType);
        var targetType = GetPropertyHandlerSetParameter(handlerInstance)?.ParameterType
            ?? (classProperty.DbType is { } dbt ? new DbTypeToClientTypeResolver().Resolve(dbt) : null)
            ?? dbField.TypeNullable();

        if (targetType.IsValueType && dbField.IsNullable)
        {
            if (Nullable.GetUnderlyingType(targetType) == null)
                targetType = typeof(Nullable<>).MakeGenericType(targetType);
        }

        /*
         * Note: The other data provider can coerce the Enum into its destination data type in the DB by default,
         *       except for PostgreSQL. The code written below is only to address the issue for this specific provider.
         */

        // Enum Handling
        if (TypeCache.Get(classProperty.PropertyInfo.PropertyType).UnderlyingType is { } underlyingType
            && underlyingType.IsEnum)
        {
            try
            {
                if (!IsPostgreSqlUserDefined(dbField))
                {
                    var dbType = classProperty.DbType ?? underlyingType.GetDbType();
                    var toType = dbType.HasValue ? new DbTypeToClientTypeResolver().Resolve(dbType.Value)! : targetType;

                    expression = ConvertEnumExpressionToTypeExpression(expression, toType);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to convert the value expression from " +
                    $"enumeration '{classProperty.PropertyInfo.PropertyType.FullName}' to type '{targetType.GetUnderlyingType()}'. {classProperty}", ex);
            }
        }

        // Auto-conversion Handling
        try
        {
            var origExpression = expression;
            expression = ConvertExpressionWithAutomaticConversion(expression, targetType);

            if (dbField?.IsIdentity == true
                && targetType.IsValueType && !TypeCache.Get(targetType).IsNullable
                && TypeCache.Get(origExpression.Type).IsNullable)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(expression.Type);

                // Don't set '0' in the identity output property
                expression = Expression.Condition(
                    Expression.Property(origExpression, nameof(Nullable<>.HasValue)),
                    Expression.Convert(expression, nullableType),
                    Expression.Constant(null, nullableType));
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to automatically convert the value expression for " +
                $"property '{classProperty.FieldName} ({classProperty.PropertyInfo.PropertyType.FullName})'. {classProperty}", ex);
        }

        // Property Handler
        try
        {
            expression = ConvertExpressionToPropertyHandlerSetExpression(
                expression, null, classProperty, TypeCache.Get(dbField?.Type).UnderlyingType);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to convert the value expression for property handler '{handlerInstance?.GetType()}'. " +
                $"{classProperty}", ex);
        }

        // Return the Value
        return ConvertExpressionToTypeExpression(expression, StaticType.Object);
    }

    private static bool IsPostgreSqlUserDefined(DbField? dbField) =>
        string.Equals(dbField?.DatabaseType, "USER-DEFINED", StringComparison.OrdinalIgnoreCase) &&
        string.Equals(dbField?.Provider, "PGSQL", StringComparison.OrdinalIgnoreCase);

    private static Expression GetObjectInstancePropertyValueExpression(ParameterExpression propertyExpression,
        Expression objectInstanceExpression,
        DbField dbField)
    {
        var methodInfo = GetMethodInfo<PropertyInfo>(x => x.GetValue(null));
        var expression = (Expression)Expression.Call(propertyExpression, methodInfo, objectInstanceExpression);

        // Property Handler
        expression = ConvertExpressionToPropertyHandlerSetExpression(expression,
            null, null, TypeCache.Get(dbField?.Type).UnderlyingType);

        // Convert to object
        return ConvertExpressionToTypeExpression(expression, StaticType.Object);
    }

    private static Expression GetDictionaryStringObjectPropertyValueExpression(Expression dictionaryInstanceExpression,
        DbField dbField)
    {
        var methodInfo = StaticType.IDictionaryStringObject.GetMethod("get_Item", [StaticType.String])!;
        var expression = (Expression)Expression.Call(dictionaryInstanceExpression, methodInfo, Expression.Constant(dbField.FieldName));

        // Property Handler
        expression = ConvertExpressionToPropertyHandlerSetExpression(expression,
            null, null, TypeCache.Get(dbField.Type).UnderlyingType);

        // Convert to object
        return ConvertExpressionToTypeExpression(expression, StaticType.Object);
    }

    private static TryExpression GetDataEntityDbParameterValueAssignmentExpression(ParameterExpression dbParameterExpression,
        Expression entityExpression,
        ParameterExpression propertyExpression,
        ClassProperty? classProperty,
        DbField dbField,
        ParameterExpression dbCommandExpression,
        IDbHelper? dbHelper)
    {
        var expression = propertyExpression.Type == StaticType.PropertyInfo
            ? GetObjectInstancePropertyValueExpression(propertyExpression, entityExpression, dbField)
            : GetEntityInstancePropertyValueExpression(entityExpression, classProperty!, dbField);

        // Nullable? -> Convert to DBNull when necessary
        if (TypeCache.Get(expression.Type) is { } returnType && returnType.HasNullValue)
        {
            expression = ConvertExpressionToDbNullExpression(expression);
        }

        // Create the method call to set the parameter
        var setValueCall = GetDbParameterValueAssignmentExpression(dbParameterExpression, expression, dbCommandExpression);

        // Use a static helper to throw the exception (to avoid closure allocation)
        var exceptionHelperMethod = GetMethodInfo(() => ThrowParameterAssignmentException("", default, default!));

        var ex = Expression.Parameter(typeof(ArgumentException), "ex");
        return Expression.TryCatch(
            Expression.Block(setValueCall, Expression.Empty()), Expression.Catch(ex,
            Expression.Call(exceptionHelperMethod,
                Expression.Constant(classProperty?.PropertyName ?? dbField?.FieldName),
                expression,
                ex)));
    }

    [DoesNotReturn]
    private static void ThrowParameterAssignmentException(string fieldName, object? value, ArgumentException ex)
    {
        if (ex is ArgumentOutOfRangeException)
            throw new ArgumentOutOfRangeException($"While setting {fieldName} to {value} ({value?.GetType()})", ex);
        else
            throw new ArgumentException($"While setting {fieldName} to {value} ({value?.GetType()})", ex);
    }

    private static BinaryExpression GetDictionaryStringObjectDbParameterValueAssignmentExpression(ParameterExpression dbParameterExpression,
        Expression dictionaryInstanceExpression,
        DbField dbField)
    {
        var expression = GetDictionaryStringObjectPropertyValueExpression(dictionaryInstanceExpression, dbField);

        // Nullable
        if (dbField?.IsNullable == true)
        {
            expression = ConvertExpressionToDbNullExpression(expression);
        }

        // Set the value
        return Expression.Assign(Expression.Property(dbParameterExpression, GetPropertyInfo<DbParameter>(x => x.Value)), expression);
    }

    private static DbType? GetDbType(ClassProperty? classProperty,
        DbField dbField)
    {
        var dbType = IsPostgreSqlUserDefined(dbField) ? DbType.Object : classProperty?.DbType;
        if (dbType is null)
        {
            var underlyingType = TypeCache.Get(dbField?.Type).UnderlyingType;
            dbType = TypeMapCache.Get(underlyingType) ?? ClientTypeToDbTypeResolver.Instance.Resolve(underlyingType);
        }
        return dbType;
    }

    private static BinaryExpression? GetDbParameterDbTypeAssignmentExpression(ParameterExpression dbParameterExpression,
        DbType? dbType)
    {
        BinaryExpression? expression = null;

        // Set the DB Type
        if (dbType is { } type)
        {
            var dbParameterDbType = Expression.Property(dbParameterExpression, GetPropertyInfo<DbParameter>(x => x.DbType));
            expression = Expression.Assign(dbParameterDbType, Expression.Constant(type));
        }

        // Return the expression
        return expression;
    }

    private static MethodCallExpression GetDbCommandCreateParameterExpression(ParameterExpression dbCommandExpression)
    {
        var dbCommandCreateParameterMethod = GetMethodInfo<DbCommand>(x => x.CreateParameter());
        return Expression.Call(dbCommandExpression, dbCommandCreateParameterMethod);
    }

    private static BinaryExpression GetDbParameterNameAssignmentExpression(Expression dbParameterExpression,
        DbField dbField,
        int entityIndex,
        IDbSetting dbSetting)
    {
        var parameterName = dbField.FieldName.AsAlphaNumeric();
        parameterName = entityIndex > 0 ? string.Concat(dbSetting.ParameterPrefix, parameterName, "_", entityIndex.ToString(CultureInfo.InvariantCulture)) :
            string.Concat(dbSetting.ParameterPrefix, parameterName);
        return GetDbParameterNameAssignmentExpression(dbParameterExpression, parameterName);
    }

    private static BinaryExpression GetDbParameterNameAssignmentExpression(Expression dbParameterExpression,
        string parameterName) =>
        GetDbParameterNameAssignmentExpression(dbParameterExpression, Expression.Constant(parameterName));

    private static BinaryExpression GetDbParameterNameAssignmentExpression(Expression dbParameterExpression,
        Expression paramaterNameExpression)
    {
        var dbParameterValueName = Expression.Property(dbParameterExpression, GetPropertyInfo<DbParameter>(x => x.ParameterName));
        return Expression.Assign(dbParameterValueName, paramaterNameExpression);
    }

    private static Expression GetDbParameterValueAssignmentExpression(Expression dbParameterExpression,
        Expression valueExpression,
        ParameterExpression? dbCommandExpression = null)
    {
        if (dbCommandExpression is null)
        {
            var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
            var dbParameterValueSet = Expression.Property(dbParameterExpression, GetPropertyInfo<DbParameter>(x => x.Value));
            var convertToDbNullMethod = GetMethodInfo(() => Converter.NullToDbNull(null));
            return Expression.Assign(dbParameterValueSet,
                Expression.Call(convertToDbNullMethod, ConvertExpressionToTypeExpression(valueExpression, StaticType.Object)));
        }
        else
        {
            return Expression.Call(
                GetMethodInfo<DbCommand>(x => x.SetValue(null!, null)),
                dbCommandExpression,
                dbParameterExpression,
                Expression.Convert(valueExpression, typeof(Object))
                );
        }
    }

    private static BinaryExpression GetDbParameterDirectionAssignmentExpression(Expression dbParameterExpression,
        ParameterDirection direction) =>
        GetDbParameterDirectionAssignmentExpression(dbParameterExpression, Expression.Constant(direction));

    private static BinaryExpression GetDbParameterDirectionAssignmentExpression(Expression dbParameterExpression,
        Expression directionExpression)
    {
        var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
        var dbParameterDirection = Expression.Property(dbParameterExpression, GetPropertyInfo<DbParameter>(x => x.Direction));
        return Expression.Assign(dbParameterDirection, directionExpression);
    }

    private static BinaryExpression GetDbParameterSizeAssignmentExpression(Expression dbParameterExpression,
        int size) =>
        GetDbParameterSizeAssignmentExpression(dbParameterExpression, Expression.Constant(size));

    private static BinaryExpression GetDbParameterSizeAssignmentExpression(Expression dbParameterExpression,
        Expression sizeExpression)
    {
        var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
        var dbParameterSize = Expression.Property(dbParameterExpression, GetPropertyInfo<DbParameter>(x => x.Size));
        return Expression.Assign(dbParameterSize, sizeExpression);
    }

    private static BinaryExpression GetDbParameterPrecisionAssignmentExpression(Expression dbParameterExpression,
        byte precision) =>
        GetDbParameterPrecisionAssignmentExpression(dbParameterExpression, Expression.Constant(precision));

    private static BinaryExpression GetDbParameterPrecisionAssignmentExpression(Expression dbParameterExpression,
        Expression precisionExpression)
    {
        var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
        var dbParameterPrecision = Expression.Property(dbParameterExpression, GetPropertyInfo<DbParameter>(x => x.Precision));
        return Expression.Assign(dbParameterPrecision, precisionExpression);
    }

    private static BinaryExpression GetDbParameterScaleAssignmentExpression(Expression dbParameterExpression,
        byte scale) =>
        GetDbParameterScaleAssignmentExpression(dbParameterExpression, Expression.Constant(scale));

    private static BinaryExpression GetDbParameterScaleAssignmentExpression(Expression dbParameterExpression,
        Expression scaleExpression)
    {
        var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
        var dbParameterScale = Expression.Property(dbParameterExpression, GetPropertyInfo<DbParameter>(p => p.Scale));
        return Expression.Assign(dbParameterScale, scaleExpression);
    }

    private static MethodCallExpression GetDbCommandParametersAddExpression(Expression dbCommandExpression,
        Expression dbParameterExpression)
    {
        var dbCommandParametersProperty = GetPropertyInfo<DbCommand>(d => d.Parameters);
        var dbParameterCollection = Expression.Property(dbCommandExpression, dbCommandParametersProperty);
        var dbParameterCollectionAddMethod = GetMethodInfo<DbParameterCollection>(x => x.Add(null!));
        return Expression.Call(dbParameterCollection, dbParameterCollectionAddMethod, dbParameterExpression);
    }

    private static MethodCallExpression GetDbParameterCollectionClearMethodExpression(MemberExpression dbParameterCollectionExpression)
    {
        var dbParameterCollectionClearMethod = GetMethodInfo<DbParameterCollection>(x => x.Clear());
        return Expression.Call(dbParameterCollectionExpression, dbParameterCollectionClearMethod);
    }

    private static BlockExpression GetPropertyFieldExpression(ParameterExpression dbCommandExpression,
        Expression entityExpression,
        FieldDirection fieldDirection,
        int entityIndex,
        IDbSetting dbSetting,
        IDbHelper? dbHelper)
    {
        var propertyListExpression = new List<Expression>();
        var propertyVariableListExpression = new List<ParameterExpression>();
        ParameterExpression? propertyVariableExpression = null;
        Expression? propertyInstanceExpression = null;
        ClassProperty? classProperty = null;
        var fieldName = fieldDirection.DbField.FieldName;

        // Set the proper assignments (property)
        if (!TypeCache.Get(entityExpression.Type).IsClassType)
        {
            var typeGetPropertyMethod = GetMethodInfo<Type>(t => t.GetProperty("", BindingFlags.Instance));
            var objectGetTypeMethod = GetMethodInfo<object>(x => x.GetType());
            propertyVariableExpression = Expression.Variable(StaticType.PropertyInfo, string.Concat("propertyVariable", fieldName));
            propertyInstanceExpression = Expression.Call(Expression.Call(entityExpression, objectGetTypeMethod),
                typeGetPropertyMethod,
                [
                    Expression.Constant(fieldName),
                    Expression.Constant(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                ]);
        }
        else
        {
            var entityProperties = PropertyCache.Get(entityExpression.Type);
            classProperty = entityProperties.GetByFieldName(fieldName);

            if (classProperty is not null)
            {
                propertyVariableExpression = Expression.Variable(classProperty.PropertyInfo.PropertyType, string.Concat("propertyVariable", fieldName));
                propertyInstanceExpression = Expression.Property(entityExpression, classProperty.PropertyInfo);
            }
            else
            {
                throw new PropertyNotFoundException(nameof(entityExpression), $"The property '{fieldName}' is not found from type '{entityExpression.Type}'. The current operation could not proceed.");
            }
        }

        // Add the variables
        if (propertyVariableExpression != null && propertyInstanceExpression != null)
        {
            propertyVariableListExpression.Add(propertyVariableExpression);
            propertyListExpression.Add(Expression.Assign(propertyVariableExpression, propertyInstanceExpression));

            // Execute the function
            var parameterAssignment = GetDataEntityParameterAssignmentExpression(dbCommandExpression,
                entityIndex,
                entityExpression,
                propertyVariableExpression,
                fieldDirection.DbField,
                classProperty,
                fieldDirection.Direction,
                dbSetting,
                dbHelper);
            propertyListExpression.Add(parameterAssignment);
        }

        // Add the property block
        return Expression.Block(propertyVariableListExpression, propertyListExpression);
    }

    private static MethodCallExpression GetDbCommandParametersClearExpression(ParameterExpression dbCommandExpression)
    {
        var dbParameterCollection = Expression.Property(dbCommandExpression, GetPropertyInfo<DbCommand>(c => c.Parameters));
        return GetDbParameterCollectionClearMethodExpression(dbParameterCollection);
    }

    private static MethodCallExpression GetListEntityIndexerExpression(Expression entitiesParameterExpression,
        Type typeOfListEntity,
        int entityIndex)
    {
        var listIndexerMethod = typeOfListEntity.GetMethod("get_Item", [StaticType.Int32])!;
        return Expression.Call(entitiesParameterExpression, listIndexerMethod,
            Expression.Constant(entityIndex));
    }

    private static ConditionalExpression ThrowIfNullAfterClassHandlerExpression(Type resultType,
        Expression expression)
    {
        var isNullExpression = Expression.Equal(Expression.Constant(null), expression);
        var exception = new ArgumentNullException($"Entity of type '{resultType}' must not be null. If you have defined a class handler, please check the 'Set' method.");
        return Expression.IfThen(isNullExpression, Expression.Throw(Expression.Constant(exception)));
    }

    private static BlockExpression GetIndexDbParameterSetterExpression(Type entityType,
        ParameterExpression dbCommandExpression,
        Expression entitiesParameterExpression,
        IEnumerable<FieldDirection> fieldDirections,
        int entityIndex,
        IDbSetting dbSetting,
        IDbHelper? dbHelper)
    {
        // Get the current instance
        var entityVariableExpression = Expression.Variable(StaticType.Object, "instance");
        var typeOfListEntity = typeof(IList<>).MakeGenericType(StaticType.Object);
        var entityParameter = (Expression)GetListEntityIndexerExpression(entitiesParameterExpression, typeOfListEntity, entityIndex);
        var entityExpressions = new List<Expression>();
        var entityVariables = new List<ParameterExpression>();

        // Class handler
        entityParameter = ConvertExpressionToClassHandlerSetExpression(dbCommandExpression, entityType, entityParameter);

        // Entity instance
        entityVariables.Add(entityVariableExpression);
        entityExpressions.Add(Expression.Assign(entityVariableExpression, entityParameter));

        // Throw if null
        entityExpressions.Add(ThrowIfNullAfterClassHandlerExpression(entityType, entityVariableExpression));

        // Iterate the input fields
        foreach (var fieldDirection in fieldDirections)
        {
            // Add the property block
            var propertyBlock = GetPropertyFieldExpression(dbCommandExpression,
                ConvertExpressionToTypeExpression(entityVariableExpression, entityType),
                fieldDirection,
                entityIndex,
                dbSetting,
                dbHelper);

            // Add to instance expression
            entityExpressions.Add(propertyBlock);
        }

        // Add to the instance block
        return Expression.Block(entityVariables, entityExpressions);
    }

    #endregion

    public static ConcurrentDictionary<(Type fromType, Type toType), Func<Expression, Expression?>> ProviderSpecificTransforms = new();
}
