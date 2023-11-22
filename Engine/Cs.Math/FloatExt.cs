namespace Cs.Math
{
    using System;

    public static class FloatExt
    {
        public static bool IsNearlyEqual(this float self, float operand, float epsilon = 0.00001f)
        {
            return Math.Abs(self - operand) < epsilon;
        }

        public static bool IsNearlyZero(this float self, float epsilon = 0.00001f)
        {
            return Math.Abs(self) < epsilon;
        }

        public static bool IsNearlyEqual(this double self, double operand, double epsilon = 0.00001)
        {
            return Math.Abs(self - operand) < epsilon;
        }

        public static bool IsNearlyZero(this double self, double epsilon = 0.00001)
        {
            return Math.Abs(self) < epsilon;
        }

        public static float Clamp(this float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}
