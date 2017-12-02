using System;

namespace decliner.Tables
{
    public class VerbPlurality
    {
        public VerbPlurality()
        {
            First = String.Empty;
            Second = String.Empty;
            Third = String.Empty;
        }

        public string First { get; set; }
        public string Second { get; set; }
        public string Third { get; set; }

        public VerbPlurality(string first, string second, string third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public override string ToString()
        {
            return $"{nameof(First)}: {First}, {nameof(Second)}: {Second}, {nameof(Third)}: {Third}";
        }
    }
}