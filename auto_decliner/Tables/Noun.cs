using decliner.Nouns;

namespace decliner.Tables
{
    /// <summary>
    ///     A class that holds a table of forms for a noun
    /// </summary>
    public class Noun
    {
        public Noun()
        {
            UseSingular = true;
            SingularCases = new Cases();
            PluralCases = new Cases();
        }

        public Noun(NounData originalNounData, Cases singularCases, Cases pluralCases, bool useSingular)
        {
            SingularCases = singularCases;
            PluralCases = pluralCases;
            UseSingular = useSingular;
        }

        public Cases SingularCases { get; set; }
        public Cases PluralCases { get; set; }
        public bool UseSingular { get; set; }

        public override string ToString()
        {
            return $"Sing: {SingularCases}, Pl: {PluralCases}, UseSing: {UseSingular}";
        }
    }
}