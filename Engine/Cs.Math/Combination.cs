namespace Cs.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Combination
    {
        private const int MaxValueRange = 100;

        public static int[] Pick(int n, int r)
        {
            if (n <= 0 || r <= 0 || n < r)
            {
                throw new Exception($"[Combination] invalid parameter. n:{n} r:{r}");
            }

            if (n > MaxValueRange)
            {
                throw new Exception($"[Combination] too large N value:{n} maxRange:{MaxValueRange}");
            }

            var hashSet = new HashSet<int>();
            while (hashSet.Count < r)
            {
                int candidate = RandomGenerator.ArrayIndex(n);
                if (hashSet.Contains(candidate) == false)
                {
                    hashSet.Add(candidate);
                }
            }

            return hashSet.ToArray();
        }
    }
}
