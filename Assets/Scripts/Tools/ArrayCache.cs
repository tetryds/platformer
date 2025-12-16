using System;

namespace Platformer.Tools
{
    public static class ArrayCache<T>
    {
        public const int DefaultSize = 32;

        private static T[] cache = new T[0];

        public static T[] Get() => Get(DefaultSize);

        public static T[] Get(int minRequirement)
        {
            if (cache.Length < minRequirement)
                Array.Resize(ref cache, minRequirement);

            return cache;
        }
    }
}
