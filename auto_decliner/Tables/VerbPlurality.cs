namespace decliner.Tables
{
    public class VerbPlurality
    {
        public VerbPlurality()
        {
            First = string.Empty;
            Second = string.Empty;
            Third = string.Empty;
        }

        public VerbPlurality(string first, string second, string third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public string First { get; set; }
        public string Second { get; set; }
        public string Third { get; set; }

        public override string ToString()
        {
            return $"{nameof(First)}: {First}, {nameof(Second)}: {Second}, {nameof(Third)}: {Third}";
        }
    }
}