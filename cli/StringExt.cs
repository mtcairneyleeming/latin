namespace cli
{
    public static class StringExt
    {
        public static string Pad(this string value, int maxLength)
        {
            return value.PadRight(maxLength).Substring(0, maxLength);
        }
    }
}