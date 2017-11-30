namespace LatinAutoDecline.Tables
{
    public struct Tense
    {
        public readonly VerbPlurality Singular;
        public readonly VerbPlurality Plural;

        public Tense(VerbPlurality singular, VerbPlurality plural)
        {
            Singular = singular;
            Plural = plural;
        }

        public override string ToString()
        {
            return $"{nameof(Singular)}: {Singular}, {nameof(Plural)}: {Plural}";
        }
    }
}