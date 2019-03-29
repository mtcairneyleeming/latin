namespace database.Helpers
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string Pad(this string value, int maxLength)
        {
            return value.PadRight(maxLength).Substring(0, maxLength);
        }
    }
}