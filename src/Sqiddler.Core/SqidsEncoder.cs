using Sqids;
using System.Numerics;

namespace Sqiddler;

public static class SqidsEncoderExtensions
{
    /// <inheritdoc cref="SqidsEncoder{T}.Decode(ReadOnlySpan{char})"/>
    public static bool TryDecode<T>(this SqidsEncoder<T> encoder, ReadOnlySpan<char> id, out T result) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        if (encoder.Decode(id) is not [T value])
        {
            result = default;
            return false;
        }
        result = value;
        return true;
    }
}
