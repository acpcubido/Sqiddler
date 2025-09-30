using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sqiddler;

public static class NullableJsonConverter
{
    public static JsonConverter Create(Type underlyingType, JsonConverter baseConverter)
    {
        return (JsonConverter)Activator.CreateInstance(typeof(NullableJsonConverter<>).MakeGenericType(underlyingType), [baseConverter])!;
    }
}
public class NullableJsonConverter<T>(JsonConverter<T> baseConverter) : JsonConverter<T?> where T : unmanaged
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else
        {
            return baseConverter.Read(ref reader, typeToConvert, options);
        }
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            baseConverter.Write(writer, value.Value, options);
        }
    }
}
