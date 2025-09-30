using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sqiddler;

public static class ArrayJsonConverter
{
    public static JsonConverter Create(Type elementType, JsonConverter baseConverter)
    {
        return (JsonConverter)Activator.CreateInstance(typeof(ArrayJsonConverter<>).MakeGenericType(elementType), [baseConverter])!;
    }
}
public class ArrayJsonConverter<T>(JsonConverter<T> baseConverter) : JsonConverter<T[]> where T :  unmanaged
{
    public override T[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }
        var result = new List<T>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            result.Add(baseConverter.Read(ref reader, typeToConvert, options));
        }
        return [.. result];
    }

    public override void Write(Utf8JsonWriter writer, T[]? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                baseConverter.Write(writer, item, options);
            }
            writer.WriteEndArray();
        }
    }
}
