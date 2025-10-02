using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Sqiddler.AspNetCore;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Sqiddler.OpenApi;

public class SqidsOpenApiTransformer : IOpenApiSchemaTransformer, IOpenApiOperationTransformer, IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (document.Components?.Schemas != null)
        {
            var sqidSchemas = document.Components.Schemas.Where(s => s.Key.StartsWith("SqidParamOf")).Select(s => s.Key).ToArray();
            foreach (var key in sqidSchemas)
            {
                //document.Components.Schemas.Remove(key);
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets properties with <see cref="JsonSqidAttribute{TSeed}"/> to be <see cref="string"/>
    /// </summary>
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonPropertyInfo != null && IsSqid(context.JsonPropertyInfo))
        {
            schema.Format = "string";
            // TODO: arrays
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Replaces parameters of type <see cref="SqidParam{TKey, TValue}"/> by <see cref="string"/>
    /// </summary>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (operation.Parameters != null)
        {
            for (var i = 0; i < operation.Parameters.Count; i++)
            {
                if (IsSqidParam(context.Description.ParameterDescriptions[i].Type))
                {
                    var schema = operation.Parameters[i].Schema!;
                    schema.Format = "string";
                    //schema.Items = null;
                }

                //    var sqidProperties = context.MethodInfo.GetParameters()
                //        .Where(p => p.GetCustomAttribute<AsParametersAttribute>() != null)
                //        .SelectMany(p => p.ParameterType.GetProperties())
                //        .Where(IsSqid);
                //    foreach (var propertyInfo in sqidProperties)
                //    {
                //        throw new NotSupportedException($"Use SqidParam for endpoint ${operation.OperationId} parameter {propertyInfo.Name} instead of JsonSqidAttribute.");
                //    }

                //    return true;
            }

        }
        return Task.CompletedTask;
    }

    private static bool IsSqid(PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType is { IsGenericType: true } converterType
            && converterType.GetGenericTypeDefinition() == typeof(SqidsJsonConverter<>);
    }
    private static bool IsSqid(JsonPropertyInfo propertyInfo)
    {
        return propertyInfo.AttributeProvider != null && propertyInfo.AttributeProvider.GetCustomAttributes(inherit: true).OfType<JsonConverterAttribute>().Any(attribute => attribute.ConverterType is { IsGenericType: true } converterType
            && converterType.GetGenericTypeDefinition() == typeof(SqidsJsonConverter<>));
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
