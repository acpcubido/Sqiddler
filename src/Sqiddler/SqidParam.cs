using System.Diagnostics;
using System.Numerics;

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

/// <summary>
/// Represents a Sqid parameter that decodes to an array of values
/// </summary>
[DebuggerDisplay("{Value,nq}")]
public readonly struct SqidArrayParam<TSeed, TValue>(TValue[] value)
    where TValue : unmanaged, IBinaryInteger<TValue>, IMinMaxValue<TValue>
{
    public readonly TValue[] Value => value;

    public static implicit operator TValue[](SqidArrayParam<TSeed, TValue> value) => value.Value;
    public static implicit operator SqidArrayParam<TSeed, TValue>(TValue[] value) => new(value);

    public static bool TryParse(string value, out SqidArrayParam<TSeed, TValue> result)
    {
        var sqids = SqidsEncoderFactory.Create<TSeed, TValue>();
        var decoded = sqids.Decode(value);
        result = new SqidArrayParam<TSeed, TValue>(decoded.ToArray());
        return true;
    }
}

/// <inheritdoc cref="SqidArrayParam{TSeed, TValue}"/>
[DebuggerDisplay("{Value,nq}")]
public readonly struct SqidArrayParam<TSeed>(int[] value)
{
    public readonly int[] Value => value;

    public static implicit operator int[](SqidArrayParam<TSeed> value) => value.Value;
    public static implicit operator SqidArrayParam<TSeed>(int[] value) => new(value);

    public static bool TryParse(string value, out SqidArrayParam<TSeed> result)
    {
        var sqids = SqidsEncoderFactory.Create<TSeed, int>();
        var decoded = sqids.Decode(value);
        result = new SqidArrayParam<TSeed>(decoded.ToArray());
        return true;
    }
}
