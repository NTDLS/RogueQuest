namespace Library.Types
{
    internal static class HgFloatExtensions
    {
        /// <summary>
        /// Clips a value to a min/max value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float Box(this float value, float minValue, float maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }
    }
}
