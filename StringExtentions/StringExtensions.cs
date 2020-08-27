using System;

namespace C_V_App.StringExtentions
{
    public static class StringExtensions
    {
        public static string MaxLength(this string value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}
