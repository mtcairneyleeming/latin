namespace database.Helpers
{
    /// <summary>
    ///     Normalise latin text:
    ///     - converts u -> v (as the other way round would look even more stupid)
    ///     - converts j -> i (as above)
    /// </summary>
    public static class TextNormaliser
    {
        public static string Fix(string input)
        {
            var output = input.Replace('j', 'i');
            output = output.Replace('u', 'v');
            if (output.EndsWith("que")) output = output.Substring(0, output.Length - 4);
            if (output.EndsWith("ve")) output = output.Substring(0, output.Length - 3);
            return output;
        }
    }
}