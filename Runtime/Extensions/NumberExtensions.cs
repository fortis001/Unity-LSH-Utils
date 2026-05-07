using UnityEngine;


namespace LSH.Utils
{
    public static class NumberExtensions
    {
        public static string ToFormatString(this float value, int pointNumber = 0)
        {
            int precision = Mathf.Max(0, pointNumber);
            return value.ToString($"N{precision}");
        }

        public static string ToFormatString(this int value)
        {
            return value.ToString("N0");
        }
    }
}

