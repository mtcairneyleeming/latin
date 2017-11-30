using LatinAutoDecline.Nouns;

namespace LatinAutoDecline.Tables
{
    /// <summary>
    /// A class that holds a table of forms for a noun
    /// </summary>
    public class Noun
    {
        public Cases? SingularCases { get; set; }
        public Cases? PluralCases { get; set; }
        public bool UseSingular { get; set; }

        public Noun()
        {
            UseSingular = true;
            SingularCases = new Cases();
            PluralCases = new Cases();
        }

        public Noun(Nouns.NounData originalNounData, Cases? singularCases, Cases pluralCases, bool useSingular)
        {
            SingularCases = singularCases;
            PluralCases = pluralCases;
            UseSingular = useSingular;
        }

        public override string ToString()
        {
            return $"Sing: {SingularCases}, Pl: {PluralCases}, UseSing: {UseSingular}";
        }
    }
}