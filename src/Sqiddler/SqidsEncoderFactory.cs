using Sqids;
using System.Numerics;

namespace Sqiddler;

public static class SqidsEncoderFactory
{
    public static SqidsEncoder<T> Create<TSeed, T>() where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        return SqidsEncoderCache<TSeed, T>.GetOrAdd();
    }

    private static class SqidsEncoderCache<TSeed, T> where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private static SqidsEncoder<T>? encoder;

        public static SqidsEncoder<T> GetOrAdd()
        {
            var result = encoder;
            if (result == null)
            {
                var options = SqidsOptionsFactory.Create<TSeed>();
                var newValue = new SqidsEncoder<T>(options);
                Interlocked.CompareExchange(ref encoder, newValue, null);
                result = encoder!;
            }
            return result;
        }
    }
}
