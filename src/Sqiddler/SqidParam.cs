using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Sqiddler.AspNetCore;

[DebuggerDisplay("{Value,nq}")]
public readonly struct SqidParam<TSeed, TValue>(TValue value)
    where TValue : unmanaged, IBinaryInteger<TValue>, IMinMaxValue<TValue>, IParsable<TValue>
{
    public readonly TValue Value => value;

    public static implicit operator TValue(SqidParam<TSeed, TValue> value) => value.Value;
    public static implicit operator SqidParam<TSeed, TValue>(TValue value) => new(value);

    public static bool TryParse(string value, out SqidParam<TSeed, TValue> result)
    {
        var sqids = SqidsEncoderFactory.Create<TSeed, TValue>();
        if (!sqids.TryDecode(value, out var result1))
        {
            result = default;
            return false;
        }
        result = result1;
        return true;
    }
}

/// <inheritdoc cref="SqidParam{TSeed, TValue}"/>
[DebuggerDisplay("{Value,nq}")]
public readonly struct SqidParam<TSeed>(int value)
{
    [JsonIgnore]
    public readonly int Value => value;

    public static implicit operator int(SqidParam<TSeed> value) => value.Value;
    public static implicit operator SqidParam<TSeed>(int value) => new(value);

    public static bool TryParse(string value, out SqidParam<TSeed> result)
    {
        var sqids = SqidsEncoderFactory.Create<TSeed, int>();
        if (!sqids.TryDecode(value, out var result1))
        {
            result = default;
            return false;
        }
        result = result1;
        return true;
    }
}
