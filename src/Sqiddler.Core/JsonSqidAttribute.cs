using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Sqiddler;

// syntactic sugar for [JsonConverter(typeof(SqidsJsonConverter<TSeed>))]

/// <summary>
/// When placed on a property, specifies the <see cref="SqidsJsonConverter{TSeed}"/> type to use.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class JsonSqidAttribute<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSeed> : JsonConverterAttribute
{
    public JsonSqidAttribute() : base(typeof(SqidsJsonConverter<TSeed>))
    {
    }
}
