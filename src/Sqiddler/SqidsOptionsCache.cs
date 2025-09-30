using Sqids;
using System.Collections.Concurrent;

namespace Sqiddler;

public static class SqidsOptionsFactory
{
    public static SqidsOptions Default { get; } = new()
    {
        MinLength = 6 // enough for 100_000_000 with default Alphabet
    };
    private static readonly ConcurrentDictionary<Type, int> minLengthBySeed = new();

    /// <param name="minLength">
    /// When using the default alphabet, reasonable values are:
    /// <list type="bullet">
    /// <item>4: enough for 100 000</item>
    /// <item>5: enough for 10 000 000</item>
    /// <item>6: enough for 100 000 000 (default)</item>
    /// <item>7: enough for <see cref="int.MaxValue"/></item>
    /// </list>
    /// </param>
    public static void Configure<TSeed>(int minLength)
    {
        minLengthBySeed[typeof(TSeed)] = minLength;
    }

    public static SqidsOptions Create<TSeed>()
    {
        return new SqidsOptions()
        {
            Alphabet = Shuffle<TSeed>(Default.Alphabet),
            BlockList = Default.BlockList,
            MinLength = minLengthBySeed.TryGetValue(typeof(TSeed), out var minLength) ? minLength : Default.MinLength
        };
    }

    private static string Shuffle<TSeed>(string alphabet)
    {
        var seed = GetHashCode(typeof(TSeed).Name);
        var random = new Random(seed);
        var chars = alphabet.ToCharArray();
        random.Shuffle(chars);
        return new string(chars);
    }

    private static int GetHashCode(ReadOnlySpan<char> values)
    {
        // see https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/

        int hash1 = (5381 << 16) + 5381;
        int hash2 = hash1;

        for (int i = 0; i < values.Length; i += 2)
        {
            hash1 = unchecked((hash1 << 5) + hash1) ^ values[i];

            if (i == values.Length - 1)
            {
                break;
            }

            hash2 = unchecked((hash2 << 5) + hash2) ^ values[i + 1];
        }

        return unchecked(hash1 + hash2 * 1566083941);
    }
}
