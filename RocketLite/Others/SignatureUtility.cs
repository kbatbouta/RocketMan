using Verse;

namespace RocketLite
{
    public static class SignatureUtility
    {
        private const int CacheSize = 31397;
        private const int A = 0x45d9f3b;
        private const int B = 0x119de1f3;
        private const int C = 0x17;

        private static readonly Thing[] owners = new Thing[CacheSize];
        private static readonly int[] signatures = new int[CacheSize];

        public static int GetSignature(this Thing thing, bool dirty = false)
        {
            var key = thing.thingIDNumber;
            unchecked
            {
                key = ((key << 30) ^ key) * A;
                key = ((key << 27) ^ key) * B;
                key = ((key << 31) ^ key) * C;
                key = (key > 0 ? key : -key) % CacheSize;
            }

            if (dirty || owners[key] != thing)
            {
                owners[key] = thing;
                return signatures[key] = Rand.Int;
            }

            return signatures[key];
        }
    }
}