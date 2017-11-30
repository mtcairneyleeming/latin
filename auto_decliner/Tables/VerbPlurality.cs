using System;

namespace LatinAutoDecline.Tables
{
    public struct VerbPlurality
    {
        public readonly string First;
        public readonly string Second; 
        public readonly string Third;

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