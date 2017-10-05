namespace LatinAutoDecline.Nouns
{
    /// <summary>
    /// A class that holds a table of forms for a noun
    /// </summary>
    public class NounTable
    {
        public Noun OriginalNoun { get; set; }
        public Cases? SingularCases { get; set; }
        public Cases? PluralCases { get; set; }
        public bool UseSingular { get; set; }

        public NounTable()
        {
            UseSingular = true;
            SingularCases = new Cases();
            PluralCases = new Cases();
        }

        public NounTable(Noun originalNoun, Cases? singularCases, Cases pluralCases, bool useSingular)
        {
            OriginalNoun = originalNoun;
            SingularCases = singularCases;
            PluralCases = pluralCases;
            UseSingular = useSingular;
        }

        public override string ToString()
        {
            return $"Noun: {OriginalNoun}, Sing: {SingularCases}, Pl: {PluralCases}, UseSing: {UseSingular}";
        }
    }
}