namespace Cs.Math
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cs.Core.Util;

    public static class RandomGenerator
    {
        public static int Range(int min, int max)
        {
            return PerThreadRandom.Instance.Next(min, max);
        }

        public static int RangeIncludeMax(int min, int max)
        {
            return PerThreadRandom.Instance.Next(min, max + 1);
        }

        public static int ArrayIndex(int count)
        {
            return PerThreadRandom.Instance.Next(count);
        }

        public static int Next(int maxValue)
        {
            return PerThreadRandom.Instance.Next(maxValue);
        }

        public static bool RollDice(int successRate, int totalRate)
        {
            return Next(totalRate) < successRate;
        }

        public static bool RollDice(int successRate, int totalRate, out int randomNumber)
        {
            randomNumber = Next(totalRate);
            return randomNumber < successRate;
        }

        public static int RandomInt()
        {
            return PerThreadRandom.Instance.Next();
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[PerThreadRandom.Instance.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        public static double NextDouble()
        {
            return PerThreadRandom.Instance.NextDouble();
        }

        public static float Range(float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentException($"[RandomGenerator] min:{min} max:{max}");
            }

            float rand = (float)PerThreadRandom.Instance.NextDouble();
            return (rand * (max - min)) + min;
        }

        public static long RandomLong()
        {
            return (long)(PerThreadRandom.Instance.NextDouble() * Int64.MaxValue);
        }

        public static ulong RandomLong(ulong min, ulong max)
        {
            byte[] buffer = new byte[8];
            PerThreadRandom.Instance.NextBytes(buffer);
            ulong longRand = buffer.DirectToUint64(0);
            ulong result = (longRand % (max - min)) + min;

            return result;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            // https://stackoverflow.com/questions/273313/randomize-a-listt
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = PerThreadRandom.Instance.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        private static class PerThreadRandom
        {
            // https://stackoverflow.com/questions/18333885/threadstatic-v-s-threadlocalt-is-generic-better-than-attribute
            private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Environment.TickCount));

            internal static Random Instance => Random.Value;
        }
    }
}
