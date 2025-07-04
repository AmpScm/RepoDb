using System.Data;
using System.Data.Common;
using System.Globalization;
using RepoDb.Attributes.Parameter;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Options;
using RepoDb.Resolvers;

namespace RepoDb.Extensions;

/// <summary>
/// Contains the extension methods for <see cref="IDbCommand"/> object.
/// </summary>
public static class DbCommandExtension
{
    #region Privates

    private static readonly ClientTypeToDbTypeResolver clientTypeToDbTypeResolver = new();
    private static readonly DbTypeToClientTypeResolver dbTypeToClientTypeResolver = new();

    #endregion

    #region CreateParameter

    /// <summary>
    /// Creates a parameter for a command object.
    /// </summary>
    /// <param name="command">The command object instance to be used.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="dbType">The database type of the parameter.</param>
    /// <returns>An instance of the newly created parameter object.</returns>
    public static IDbDataParameter CreateParameter(this IDbCommand command,
        string name,
        object? value,
        DbType? dbType) =>
        CreateParameter(command, name, value, dbType, null);

    /// <summary>
    /// Creates a parameter for a command object.
    /// </summary>
    /// <param name="command">The command object instance to be used.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="dbType">The database type of the parameter.</param>
    /// <param name="parameterDirection">The direction of the parameter.</param>
    /// <returns>An instance of the newly created parameter object.</returns>
    public static IDbDataParameter CreateParameter(this IDbCommand command,
        string name,
        object? value,
        DbType? dbType,
        ParameterDirection? parameterDirection)
    {
        // Create the parameter
        var parameter = command.CreateParameter();

        // Set the values
        parameter.ParameterName = name.AsParameter(index: 0, quote: false, DbSettingMapper.Get(command.Connection!));

        if (DbHelperMapper.Get(command.Connection!) is BaseDbHelper dbh)
        {
            value = dbh.ParameterValueToDb(value);
        }

        parameter.Value = value ?? DBNull.Value;

        // The DB Type is auto set when setting the values
        if (dbType != null)
        {
            // Prepare() requires an explicit assignment, weird Microsoft
            parameter.DbType = dbType.Value;
        }

        // Set the direction
        if (parameterDirection != null)
        {
            parameter.Direction = parameterDirection.Value;
        }

        // Table-Valued Parameter
        EnsureTableValueParameter(parameter);

        // Return the parameter
        return parameter;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="parameter"></param>
    internal static void EnsureTableValueParameter(IDbDataParameter parameter)
    {
        if (parameter == null || parameter.Value is DataTable table == false)
        {
            return;
        }

        var property = parameter.GetType().GetProperty("TypeName");
        if (property == null)
        {
            return;
        }

        property.SetValue(parameter, table.TableName);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="commandArrayParametersText"></param>
    internal static void CreateParametersFromArray(
        this DbCommand command,
        DbConnection connection,
        IDbTransaction? transaction,
        CommandArrayParametersText commandArrayParametersText)
    {
        if (commandArrayParametersText?.CommandArrayParameters?.Count is not > 0)
        {
            return;
        }
        var dbSetting = command.Connection!.GetDbSetting();
        foreach (var commandArrayParameter in commandArrayParametersText.CommandArrayParameters)
        {
            CreateParametersFromArray(command,
                connection,
                transaction,
                commandArrayParameter,
                commandArrayParametersText.DbType, dbSetting);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="commandArrayParameter"></param>
    /// <param name="dbType"></param>
    /// <param name="dbSetting"></param>
    private static void CreateParametersFromArray(this DbCommand command,
        DbConnection connection,
        IDbTransaction? transaction,
        CommandArrayParameter commandArrayParameter,
        DbType? dbType,
        IDbSetting dbSetting)
    {
        var values = commandArrayParameter.Values.AsTypedSet();

        if (values.Count == 0)
        {
            command.Parameters.Add(
                command.CreateParameter(commandArrayParameter.ParameterName, null, dbType));
        }
        else if (values.Count > 5 && dbSetting.UseArrayParameterTreshold < values.Count
            && command.Connection?.GetDbHelper().CreateTableParameter(connection, transaction, null,
            values, commandArrayParameter.ParameterName) is { } tableParameter)
        {
            command.Parameters.Add(tableParameter);
        }
        else
        {
            int i = 0;
            foreach (var value in values)
            {
                var name = string.Concat(commandArrayParameter.ParameterName, i.ToString(CultureInfo.InvariantCulture));
                dbType ??= value?.GetType().GetDbType();
                command.Parameters.Add(
                    command.CreateParameter(name, value, dbType));

                i++;
            }
        }
    }

    /// <summary>
    /// Creates a parameter from object by mapping the property from the target entity type.
    /// </summary>
    /// <param name="command">The command object to be used.</param>
    /// <param name="param">The object to be used when creating the parameters.</param>
    public static void CreateParameters(this IDbCommand command,
        object param) =>
        CreateParameters(command, param, param?.GetType());

    /// <summary>
    /// Creates a parameter from object by mapping the property from the target entity type.
    /// </summary>
    /// <param name="command">The command object to be used.</param>
    /// <param name="param">The object to be used when creating the parameters.</param>
    /// <param name="entityType">The type of the data entity.</param>
    public static void CreateParameters(this IDbCommand command,
        object? param,
        Type? entityType) =>
        CreateParameters(command, param, null, entityType, null);

    internal static void CreateParameters(this IDbCommand command,
        object? param,
        HashSet<string>? propertiesToSkip,
        Type? entityType,
        DbFieldCollection? dbFields = null)
    {
        // Check
        if (param == null)
        {
            return;
        }

        // IDictionary<string, object>
        switch (param)
        {
            case IDictionary<string, object> objects:
                CreateParameters(command, objects, propertiesToSkip, dbFields);
                break;

            // QueryField
            case QueryField field:
                command.CreateParameters(field, propertiesToSkip, entityType, dbFields);
                break;

            // IEnumerable<QueryField>
            case IEnumerable<QueryField> fields:
                command.CreateParameters(fields, propertiesToSkip, entityType, dbFields);
                break;

            // QueryGroup
            case QueryGroup group:
                CreateParameters(command, group, propertiesToSkip, entityType, dbFields);
                break;

            // Other
            default:
                CreateParametersInternal(command, param, propertiesToSkip, entityType, dbFields);
                break;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="size"></param>
    /// <param name="classProperty"></param>
    /// <param name="dbField"></param>
    /// <param name="parameterDirection"></param>
    /// <param name="dbType"></param>
    /// <param name="fallbackType"></param>
    /// <returns></returns>
    private static IDbDataParameter CreateParameter(IDbCommand command,
        string name,
        object? value,
        int? size,
        ClassProperty? classProperty,
        DbField? dbField,
        ParameterDirection? parameterDirection,
        DbType? dbType,
        Type? fallbackType)
    {
        if (value is IDbDataParameter parameter)
        {
            // If the value is already a parameter, just set the name and return it
            parameter.ParameterName = name.AsParameter(index: 0, quote: false, DbSettingMapper.Get(command.Connection!));
            return parameter;
        }

        var valueType = TypeCache.Get(value?.GetType() ?? classProperty?.PropertyInfo.PropertyType).GetUnderlyingType();

        if (valueType?.IsEnum == true)
        {
            return CreateParameterForEnum(command,
                valueType,
                name,
                value,
                size,
                classProperty,
                dbField,
                parameterDirection,
                dbType);
        }
        else
        {
            return CreateParameterForNonEnum(command,
                valueType,
                name,
                value,
                size,
                classProperty,
                dbField,
                parameterDirection,
                dbType,
                fallbackType);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="valueType"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="size"></param>
    /// <param name="classProperty"></param>
    /// <param name="dbField"></param>
    /// <param name="parameterDirection"></param>
    /// <param name="dbType"></param>
    /// <param name="fallbackType"></param>
    /// <returns></returns>
    private static IDbDataParameter CreateParameterForNonEnum(IDbCommand command,
        Type? valueType,
        string name,
        object? value,
        int? size,
        ClassProperty? classProperty,
        DbField? dbField,
        ParameterDirection? parameterDirection,
        DbType? dbType,
        Type? fallbackType)
    {
        // DbType
        valueType ??= TypeCache.Get(dbField?.Type).GetUnderlyingType() ?? fallbackType;
        dbType ??= classProperty?.GetDbType() ?? (dbField?.Type ?? fallbackType ?? valueType)?.GetDbType();

        // Create the parameter
        var parameter = command.CreateParameter(name, value, dbType, parameterDirection);

        // Property Handler
        InvokePropertyHandler(classProperty, parameter, ref valueType, ref value);

        // Automatic Conversion
        bool converted = AutomaticConvert(dbField, ref valueType, ref value);
        parameter.Value = (value ?? DBNull.Value);
        if (converted)
        {
            parameter.DbType = clientTypeToDbTypeResolver.Resolve(valueType!)!.Value;
        }

        // Set the size
        if ((size ?? dbField?.Size) is { } parameterSize)
        {
            parameter.Size = parameterSize;
        }

        // Parameter values
        InvokePropertyValueAttributes(parameter, GetPropertyValueAttributes(classProperty, valueType));

        // Return the parameter
        return parameter;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="valueType"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="size"></param>
    /// <param name="classProperty"></param>
    /// <param name="dbField"></param>
    /// <param name="parameterDirection"></param>
    /// <param name="dbType"></param>
    /// <returns></returns>
    private static IDbDataParameter CreateParameterForEnum(IDbCommand command,
        Type? valueType,
        string name,
        object? value,
        int? size,
        ClassProperty? classProperty,
        DbField? dbField,
        ParameterDirection? parameterDirection,
        DbType? dbType)
    {
        // DbType
        dbType ??= IsPostgreSqlUserDefined(dbField) ? default :
            classProperty?.GetDbType() ??
            valueType.GetDbType() ??
            (dbField != null ? clientTypeToDbTypeResolver.Resolve(dbField.Type) : null) ??
            (DbType?)GlobalConfiguration.Options.EnumDefaultDatabaseType;

        // Create the parameter
        if (dbType == DbType.String)
        {
            // The database needs the value as string for an operation

            value = value?.ToString();
        }

        var parameter = command.CreateParameter(name, value, dbType, parameterDirection);

        // Property handler
        InvokePropertyHandler(classProperty, parameter, ref valueType, ref value);

        // Set the parameter value (in case)
        parameter.Value = (value ?? DBNull.Value);

        // Set the size
        if ((size ?? dbField?.Size) is { } paramSize)
            parameter.Size = paramSize;

        // Type map attributes
        InvokePropertyValueAttributes(parameter, GetPropertyValueAttributes(classProperty, valueType));

        // Return the parameter
        return parameter;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static IDbDataParameter? CreateParameterIf(string name,
        object? value)
    {
        if (value is IDbDataParameter parameter)
        {
            parameter.ParameterName = name;
            return parameter;
        }

        return null;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="param"></param>
    /// <param name="propertiesToSkip"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    private static void CreateParametersInternal(IDbCommand command,
        object param,
        HashSet<string>? propertiesToSkip,
        Type? entityType,
        DbFieldCollection? dbFields = null)
    {
        var type = param.GetType();

        // Check
        if (type.IsGenericType && type.GetGenericTypeDefinition() == StaticType.Dictionary)
        {
            throw new InvalidParameterException("The supported type of dictionary object must be of type IDictionary<string, object>.");
        }

        // Variables
        var entityClassProperties = entityType != null ? PropertyCache.Get(entityType) : default;
        var paramClassProperties = TypeCache.Get(type).IsClassType() ? PropertyCache.Get(type) : type.GetClassProperties();

        // Skip
        if (propertiesToSkip != null)
        {
            paramClassProperties = paramClassProperties.Where(p => propertiesToSkip.Contains(p.PropertyInfo.Name) == false);
        }

        // Iterate
        foreach (var paramClassProperty in paramClassProperties)
        {
            var entityClassProperty = (entityType == paramClassProperty.DeclaringType) ?
                paramClassProperty :
                entityClassProperties.GetByFieldName(paramClassProperty.FieldName);
            var name = paramClassProperty
                .FieldName
                .AsUnquoted(command.Connection!.GetDbSetting());
            var dbField = GetDbField(name, dbFields);
            var value = paramClassProperty.PropertyInfo.GetValue(param);
            var parameter = CreateParameterIf(name, value) ??
                CreateParameter(command,
                    name,
                    value,
                    dbField?.Size,
                    (entityClassProperty ?? paramClassProperty),
                    dbField,
                    null,
                    null,
                    null);
            command.Parameters.Add(parameter);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="dictionary"></param>
    /// <param name="propertiesToSkip"></param>
    /// <param name="dbFields"></param>
    private static void CreateParameters(IDbCommand command,
        IDictionary<string, object> dictionary,
        HashSet<string>? propertiesToSkip,
        DbFieldCollection? dbFields = null)
    {
        var kvps = dictionary.Where(kvp =>
            propertiesToSkip?.Contains(kvp.Key) != true);

        // Iterate the key value pairs
        foreach (var kvp in kvps)
        {
            var dbField = GetDbField(kvp.Key, dbFields);
            var value = kvp.Value;
            ClassProperty? classProperty = null;

            try
            {
                // CommandParameter
                if (kvp.Value is CommandParameter commandParameter)
                {
                    value = commandParameter.Value;
                    dbField ??= GetDbField(commandParameter.Field.FieldName, dbFields);
                    classProperty = PropertyCache.Get(commandParameter.MappedToType, commandParameter.Field.FieldName, true);
                }
                var parameter = CreateParameterIf(kvp.Key, value) ??
                    CreateParameter(command,
                        kvp.Key,
                        value,
                        dbField?.Size,
                        classProperty,
                        dbField,
                        null,
                        null,
                        null);
                command.Parameters.Add(parameter);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"While setting {kvp.Key} to {value} ({value?.GetType()})", ex);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="queryGroup"></param>
    /// <param name="propertiesToSkip"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    internal static void CreateParameters(IDbCommand command,
        QueryGroup? queryGroup,
        HashSet<string>? propertiesToSkip,
        Type? entityType,
        DbFieldCollection? dbFields = null)
    {
        if (queryGroup == null)
        {
            return;
        }
        CreateParameters(command, queryGroup.GetFields(true), propertiesToSkip, entityType, dbFields);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="queryFields"></param>
    /// <param name="propertiesToSkip"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    internal static void CreateParameters(this IDbCommand command,
        IEnumerable<QueryField>? queryFields,
        HashSet<string>? propertiesToSkip,
        Type? entityType,
        DbFieldCollection? dbFields = null)
    {
        if (queryFields == null)
        {
            return;
        }

        // Filter the query fields
        var filteredQueryFields = queryFields
            .Where(qf => propertiesToSkip?.Contains(qf.Field.FieldName) != true);

        // Iterate the filtered query fields
        foreach (var queryField in filteredQueryFields)
        {
            if (queryField.Operation is Operation.In or Operation.NotIn)
            {
                var dbField = GetDbField(queryField.Field.FieldName, dbFields);
                CreateParametersForInOperation(command, queryField, dbField);
            }
            else if (queryField.Operation is Operation.Between or Operation.NotBetween)
            {
                var dbField = GetDbField(queryField.Field.FieldName, dbFields);
                CreateParametersForBetweenOperation(command, queryField, dbField);
            }
            else if (queryField.Operation is Operation.IsNotNull or Operation.IsNull)
            {
                // No value needed
            }
            else
            {
                CreateParameters(command, queryField, null, entityType, dbFields);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="queryField"></param>
    /// <param name="propertiesToSkip"></param>
    /// <param name="entityType"></param>
    /// <param name="dbFields"></param>
    private static void CreateParameters(this IDbCommand command,
        QueryField queryField,
        HashSet<string>? propertiesToSkip,
        Type? entityType,
        DbFieldCollection? dbFields = null)
    {
        if (queryField == null)
        {
            return;
        }

        var fieldName = queryField.Field.FieldName;

        // Skip
        if (propertiesToSkip?.Contains(fieldName) == true)
        {
            return;
        }

        // Variables
        var dbField = GetDbField(fieldName, dbFields);
        var value = queryField.Parameter.Value;
        var classProperty = PropertyCache.Get(entityType, queryField.Field, true);
        var (direction, fallbackType, size) = queryField is DirectionalQueryField n ?
            (
                n.Direction,
                n.Parameter.DbType.HasValue ?
                    dbTypeToClientTypeResolver.Resolve(n.Parameter.DbType.Value) : null,
                n.Size ?? dbField?.Size
            ) : default;

        // Create the parameter
        var parameter = CreateParameterIf(queryField.Parameter.Name, value) ??
            CreateParameter(command,
                queryField.Parameter.Name,
                value,
                size,
                classProperty,
                dbField,
                direction,
                queryField.Parameter.DbType,
                fallbackType);
        command.Parameters.Add(parameter);

        // Set the parameter
        queryField.DbParameter = parameter;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="queryField"></param>
    /// <param name="dbField"></param>
    private static void CreateParametersForInOperation(this IDbCommand command,
        QueryField queryField,
        DbField? dbField = null)
    {
        var enumerable = (System.Collections.IEnumerable?)queryField.Parameter.Value;
        if (!queryField.TableParameterMode)
        {
            var values = (queryField.Parameter.Value as System.Collections.IEnumerable)?.AsTypedSet();
            if (!(values?.Count > 0))
                return;

            int i = 0;
            foreach (var value in values)
            {
                var name = string.Concat(queryField.Parameter.Name, "_In_", i.ToString(CultureInfo.InvariantCulture));
                var parameter = CreateParameter(command,
                    name,
                    value,
                    dbField?.Size,
                    null,
                    dbField,
                    null,
                    queryField.Parameter.DbType,
                    null);
                command.Parameters.Add(parameter);
                i++;
            }

            int mp = QueryField.RoundUpInLength(i);
            while (i < mp)
            {
                var name = string.Concat(queryField.Parameter.Name, "_In_", i.ToString(CultureInfo.InvariantCulture));
                var parameter = CreateParameter(command,
                    name,
                    null,
                    dbField?.Size,
                    null,
                    dbField,
                    null,
                    queryField.Parameter.DbType,
                    null);
                command.Parameters.Add(parameter);
                i++;
            }
        }
        else
        {
            var connection = command.Connection ?? throw new InvalidOperationException("The command connection cannot be null.");
            var param = connection.GetDbHelper().CreateTableParameter(command.Connection!, null, null, enumerable ?? Enumerable.Empty<object>(), queryField.Parameter.Name.AsParameter(0, false, connection.GetDbSetting(), suffix: "_In_"));
            command.Parameters.Add(param);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="queryField"></param>
    /// <param name="dbField"></param>
    private static void CreateParametersForBetweenOperation(this IDbCommand command,
        QueryField queryField,
        DbField? dbField = null)
    {
        var values = (queryField?.Parameter.Value as System.Collections.IEnumerable)?.WithType<object>().ToList();

        if (values?.Count == 2)
        {
            // Left
            var leftParameter = CreateParameter(command,
                string.Concat(queryField!.Parameter.Name, "_Left"),
                values[0],
                dbField?.Size,
                null, dbField,
                null,
                queryField.Parameter.DbType,
                null);
            command.Parameters.Add(leftParameter);

            // Right
            var rightParameter = CreateParameter(command,
                string.Concat(queryField.Parameter.Name, "_Right"),
                values[1],
                dbField?.Size,
                null,
                dbField,
                null,
                queryField.Parameter.DbType,
                null);
            command.Parameters.Add(rightParameter);
        }
        else
        {
            throw new InvalidParameterException("The values for 'Between' and 'NotBetween' operations must be 2.");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="classProperty"></param>
    /// <param name="parameter"></param>
    /// <param name="valueType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static void InvokePropertyHandler(ClassProperty? classProperty,
        IDbDataParameter parameter,
        ref Type? valueType,
        ref object? value)
    {
        var propertyHandler = classProperty?.GetPropertyHandler() ??
            (valueType == null ? null : PropertyHandlerCache.Get<object>(valueType));

        if (propertyHandler != null)
        {
            var propertyHandlerSetMethod = Reflection.Compiler.GetPropertyHandlerInterfaceOrHandlerType(propertyHandler)?.GetMethod(nameof(IPropertyHandler<object, object>.Set))!;
            value = propertyHandlerSetMethod
                .Invoke(propertyHandler, new[] { value,
                    PropertyHandlerSetOptions.Create(parameter, classProperty!) });
            valueType = TypeCache.Get(propertyHandlerSetMethod.ReturnType).GetUnderlyingType();
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbField"></param>
    /// <returns></returns>
    private static bool IsPostgreSqlUserDefined(DbField? dbField) =>
        string.Equals(dbField?.DatabaseType, "USER-DEFINED", StringComparison.OrdinalIgnoreCase) &&
        string.Equals(dbField?.Provider, "PGSQL", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///
    /// </summary>
    /// <param name="classProperty"></param>
    /// <param name="fallbackType"></param>
    /// <returns></returns>
    private static IEnumerable<PropertyValueAttribute>? GetPropertyValueAttributes(ClassProperty? classProperty,
        Type? fallbackType) =>
        classProperty?.GetPropertyValueAttributes() ?? fallbackType?.GetPropertyValueAttributes();

    /// <summary>
    ///
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="attributes"></param>
    private static void InvokePropertyValueAttributes(IDbDataParameter parameter,
        IEnumerable<PropertyValueAttribute>? attributes)
    {
        if (attributes?.Any() != true)
        {
            return;
        }

        // In RepoDb, the only way the parameter has '@_' is when the time you call the QueryField.IsForUpdate()
        // method and it is only happening on update operations.
        var isForUpdate = parameter.ParameterName.StartsWith('_') || parameter.ParameterName.StartsWith("@_", StringComparison.Ordinal);

        foreach (var attribute in attributes)
        {
            var exclude = isForUpdate &&
                (
                    attribute is NameAttribute ||
                    string.Equals(nameof(IDbDataParameter.ParameterName), attribute.PropertyName, StringComparison.OrdinalIgnoreCase)
                );

            if (exclude)
            {
                continue;
            }
            attribute.SetValue(parameter);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dbField"></param>
    /// <param name="valueType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool AutomaticConvert(DbField? dbField,
        ref Type? valueType,
        ref object? value)
    {
        if (valueType != null && dbField != null && dbField?.Type != null)
        {
            var dbFieldType = TypeCache.Get(dbField.Type).GetUnderlyingType();

            if (dbFieldType != valueType)
            {
                if (value != null)
                {
                    value = AutomaticConvert(value, valueType, dbFieldType);
                }

                valueType = dbFieldType;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="dbFields"></param>
    /// <returns></returns>
    private static DbField? GetDbField(string fieldName,
        DbFieldCollection? dbFields)
    {
        if (dbFields is null || dbFields.IsEmpty()) return null;

        var fieldNameSpan = fieldName.AsSpan();

        if (fieldNameSpan.IsEmpty || fieldNameSpan.IsWhiteSpace())
        {
            return null;
        }

        var index = fieldNameSpan.IndexOf("_In_".AsSpan(), StringComparison.OrdinalIgnoreCase);

        if (index >= 0)
        {
            fieldNameSpan = fieldNameSpan.Slice(0, index);
        }

        return dbFields.GetByFieldName(fieldNameSpan.ToString());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromType"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    private static object? AutomaticConvert(object? value,
        Type fromType,
        Type targetType)
    {
        if (fromType == null || targetType == null || fromType == targetType)
        {
            return value;
        }
        if (fromType == StaticType.String && targetType == StaticType.Guid)
        {
            return AutomaticConvertStringToGuid(value);
        }
        else if (fromType == StaticType.Guid && targetType == StaticType.String)
        {
            return AutomaticConvertGuidToString(value);
        }
        else if (fromType == StaticType.DateTimeOffset && targetType == StaticType.DateTime && value is DateTimeOffset dto)
        {
            return dto.DateTime;
        }
        else if (targetType == StaticType.DateTime && value is DateTime dt)
        {
            return dt;
        }
        else if (fromType == StaticType.DateTime && targetType == StaticType.String)
        {
            return ((DateTime)value!).ToString("o", CultureInfo.InvariantCulture);
        }
        else if (fromType == StaticType.DateTimeOffset && targetType == StaticType.String)
        {
            return ((DateTimeOffset)value!).ToString("o", CultureInfo.InvariantCulture);
        }
#if NET
        else if (fromType == StaticType.DateOnly && targetType == StaticType.DateTime)
        {
            return AutomaticConvertDateOnlyToDateTime(value);
        }
        else if (fromType == StaticType.TimeOnly && targetType == StaticType.String)
        {
            return ((TimeOnly)value!).ToString("o", CultureInfo.InvariantCulture);
        }
        else if (fromType == StaticType.DateOnly && targetType == StaticType.String)
        {
            return ((TimeOnly)value!).ToString("d", CultureInfo.InvariantCulture);
        }
#endif
        else
        {
            if (value == DBNull.Value)
            {
                if (TypeCache.Get(targetType).HasNullValue())
                    return null;
                else
                    return Activator.CreateInstance(targetType);
            }
            else if (targetType == StaticType.Object)
            {
                return value;
            }

            try
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException($"While converting from {value?.GetType().FullName} to {targetType.FullName}: " + e.Message, e);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static object? AutomaticConvertStringToGuid(object? value)
    {
        if (value is string { } str)
        {
            value = Guid.Parse(str);
        }
        return value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string? AutomaticConvertGuidToString(object? value) =>
        value?.ToString();

#if NET
    private static DateTime? AutomaticConvertDateOnlyToDateTime(object? value) =>
        (value is DateOnly dateOnly ? dateOnly.ToDateTime(default(TimeOnly)) : null);
#endif
    #endregion
}
