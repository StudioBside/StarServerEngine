namespace UnitTest
{
    internal static class AssertUtil
    {
        public static bool IsBetween(this int self, int min, int max)
        {
            return min <= self && self <= max;
        }
    }
}
