using System;

namespace Mirle.Stocker.R46YP320.Extensions
{
    public static class StringHelper
    {
        public static string TruncateRight(this string value, int maxLength)
        {
            maxLength = maxLength > 0 ? maxLength : 0;
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var newLength = Math.Min(value.Length, maxLength);
            return value.Substring(value.Length - newLength, newLength);
        }
    }
}