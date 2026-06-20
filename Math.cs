namespace SealedFates
{
    public class Math
    {
        public Int128 Clamp(Int128 value, Int128 min, Int128 max)
        {
            if (value > max)
                value = max;
            if (value < min)
                value = min;

            return value;
        }

        public Int128 Min(Int128 a, Int128 b)
        {
            if (a == b)
                return a;

            return a > b ? b : a;
        }
        public Int128 Max(Int128 a, Int128 b)
        {
            if (a == b)
                return a;

            return a > b ? a : b;
        }

        public Int256 Clamp(Int256 value, Int256 min, Int256 max)
        {
            if (value > max)
                value = max;
            if (value < min)
                value = min;

            return value;
        }

        public Int256 Min(Int256 a, Int256 b)
        {
            if (a == b)
                return a;

            return a > b ? b : a;
        }
        public Int256 Max(Int256 a, Int256 b)
        {
            if (a == b)
                return a;

            return a > b ? a : b;
        }
    }
}