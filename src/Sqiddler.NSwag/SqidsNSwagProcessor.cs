using Microsoft.AspNetCore.Http;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Sqiddler.AspNetCore;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Sqiddler.NSwag;

public class SqidsNSwagProcessor : ISchemaProcessor, IOperationProcessor, IDocumentProcessor
{
    /// <summary>
    /// Removes (unused) schemas for <see cref="SqidParam{TSeed}"/>
    /// </summary>
    public void Process(DocumentProcessorContext context)
    {
        var sqidSchemas = context.Document.Components.Schemas.Where(s => s.Key.StartsWith("SqidParamOf")).Select(s => s.Key).ToArray();
        foreach (var key in sqidSchemas)
        {
            context.Document.Components.Schemas.Remove(key);
        }
    }

    /// <summary>
    /// Sets properties with <see cref="JsonSqidAttribute{TSeed}"/> to be <see cref="string"/>
    /// </summary>
    public void Process(SchemaProcessorContext context)
    {
        if (context.Schema.HasReference)
        {
            return;
        }

        // PascalCase vs. camelCase
        var propertiesByName = context.Schema.Properties.ToDictionary(p => p.Key, StringComparer.OrdinalIgnoreCase);
        var sqidProperties = context.ContextualType.Properties.Where(p => IsSqid(p.PropertyInfo));
        foreach (var propertyInfo in sqidProperties)
        {
            var propertySchema = propertiesByName[propertyInfo.Name].Value;
            SetObjectType(propertySchema, propertyInfo.PropertyType.TypeInfo, JsonObjectType.String);
        }
    }

    /// <summary>
    /// Replaces parameters of type <see cref="SqidParam{TKey, TValue}"/> by <see cref="string"/>
    /// </summary>
    public bool Process(OperationProcessorContext context)
    {
        var sqidParameters = context.MethodInfo.GetParameters()
            .Where(p => IsSqidParam(p.ParameterType));
        foreach (var parameterInfo in sqidParameters)
        {
            var parameter = context.OperationDescription.Operation.Parameters.First(p => p.Name == parameterInfo.Name);
            if (parameterInfo.ParameterType.IsArray)
            {
                parameter.Schema = context.SchemaGenerator.Generate(typeof(string[]), context.SchemaResolver);
            }
            else
            {
                parameter.Schema = context.SchemaGenerator.Generate(typeof(string), context.SchemaResolver);
            }
        }

        var sqidProperties = context.MethodInfo.GetParameters()
            .Where(p => p.GetCustomAttribute<AsParametersAttribute>() != null)
            .SelectMany(p => p.ParameterType.GetProperties())
            .Where(IsSqid);
        foreach (var propertyInfo in sqidProperties)
        {
            throw new NotSupportedException($"Use SqidParam for endpoint ${context.MethodInfo.Name} parameter {propertyInfo.Name} instead of JsonSqidAttribute.");
        }

        return true;
    }

    private static void SetObjectType(JsonSchema propertySchema, Type propertyType, JsonObjectType type)
    {
        if (propertyType.IsArray)
        {
            propertySchema.Item!.Type = type;
        }
        else
        {
            propertySchema.Type = type;
        }
    }

    private static bool IsSqid(PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType is { IsGenericType: true } converterType
            && converterType.GetGenericTypeDefinition() == typeof(SqidsJsonConverter<>);
    }
    private static bool IsSqidParam(Type type)
    {
        if (type.IsArray && IsSqidParam(type.GetElementType()!))
        {
            return IsSqidParam(type.GetElementType()!);
        }
        return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(SqidParam<,>) || type.GetGenericTypeDefinition() == typeof(SqidParam<>));
    }
}
