namespace decliner.Tables
{
    public class Tense
    {
        public VerbPlurality Plural{ get; set; }
        public VerbPlurality Singular{ get; set; }

        public Tense(VerbPlurality singular, VerbPlurality plural)
        {
            Singular = singular;
            Plural = plural;
        }

        public Tense()
        {
            Singular = new VerbPlurality();
            Plural = new VerbPlurality();
        }


        public override string ToString()
        {
            return $"{nameof(Singular)}: {Singular}, {nameof(Plural)}: {Plural}";
        }
    }
}