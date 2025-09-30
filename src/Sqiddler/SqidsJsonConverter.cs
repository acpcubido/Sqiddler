using Sqids;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sqiddler;


public class SqidsJsonConverter<TSeed> : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return true;
    }
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert.IsArray)
        {
            var elementType = typeToConvert.GetElementType()!;
            var baseConverter = SqidJsonConverter.Create<TSeed>(elementType);
            return ArrayJsonConverter.Create(elementType, baseConverter);
        }
        if (Nullable.GetUnderlyingType(typeToConvert) is { } underlyingType)
        {
            var baseConverter = SqidJsonConverter.Create<TSeed>(underlyingType);
            return NullableJsonConverter.Create(underlyingType, baseConverter);
        }
        return SqidJsonConverter.Create<TSeed>(typeToConvert);
    }
}
public static class SqidJsonConverter
{
    public static JsonConverter Create<TSeed>(Type typeToConvert)
    {
        return (JsonConverter)Activator.CreateInstance(typeof(SqidJsonConverter<,>).MakeGenericType(typeof(TSeed), typeToConvert))!;
    }
}
public class SqidJsonConverter<TSeed, T> : JsonConverter<T> where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
{
    private readonly SqidsEncoder<T> sqids = SqidsEncoderFactory.Create<TSeed, T>();

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }
        var id = reader.GetString();
        if (!sqids.TryDecode(id, out var result))
        {
            throw new JsonException();
        }
        return result;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var id = sqids.Encode(value);
        writer.WriteStringValue(id);
    }
}
