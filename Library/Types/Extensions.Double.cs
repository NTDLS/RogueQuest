namespace Library.Types
{
    internal static class HgDoubleExtensions
    {
        /// <summary>
        /// Degrees 0-360 -> 0 to 180 (right) and 0 to -180 (left).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double DegreesNormalized(this double value)
        {
            return (value + 180) % 360 - 180;
        }

        /// <summary>
        /// Degrees 0-Infinite -> 0 to 360
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double DegreesNormalized360(this double value)
        {
            return ((dynamic)value + 360) % 360;
        }

        public static bool IsNotBetween(this double value, double minValue, double maxValue)
        {
            return !value.IsBetween(minValue, maxValue);
        }

        public static bool IsNotBetween(this double? value, double minValue, double maxValue)
        {
            return !value.IsBetween(minValue, maxValue);
        }

        public static bool IsBetween(this double value, double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                return value >= maxValue && value <= minValue;
            }
            return value >= minValue && value <= maxValue;
        }

        public static bool IsBetween(this double? value, double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                return value >= maxValue && value <= minValue;
            }
            return value >= minValue && value <= maxValue;
        }

        /// <summary>
        /// Clips a value to a min/max value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static double Box(this double value, double minValue, double maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }

        /// <summary>
        /// Take a value divides it by two and makes it negative if it over a given threshold
        /// </summary>
        /// <param name="value"></param>
        /// <param name="at"></param>
        /// <returns></returns>
        public static double SplitToNegative(this double value, double threshold)
        {
            value /= 2.0;

            if (value > threshold)
            {
                value *= -1;
            }

            return value;
        }
    }
}
