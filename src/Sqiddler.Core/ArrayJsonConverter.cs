using System.Text.Json;
using System.Text.Json.Serialization;
using Sqids;

namespace Sqiddler;

public static class ArrayJsonConverter
{
    public static JsonConverter Create<TSeed>(Type elementType)
    {
        return (JsonConverter)Activator.CreateInstance(typeof(ArrayJsonConverter<,>).MakeGenericType(typeof(TSeed), elementType))!;
    }
}
public class ArrayJsonConverter<TSeed, T> : JsonConverter<T[]> where T : unmanaged, System.Numerics.IBinaryInteger<T>, System.Numerics.IMinMaxValue<T>
{
    private readonly SqidsEncoder<T> sqids = SqidsEncoderFactory.Create<TSeed, T>();

    public override T[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }
        var id = reader.GetString();
        var decoded = sqids.Decode(id);
        return decoded.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, T[]? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            var id = sqids.Encode(value);
            writer.WriteStringValue(id);
        }
    }
}
