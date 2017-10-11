namespace LatinAutoDecline.Nouns
{
    /// <summary>
    /// A class that holds a table of forms for a noun
    /// </summary>
    public class NounTable
    {
        public Noun OriginalNoun { get; set; }
        public CaseTable? SingularCaseTable { get; set; }
        public CaseTable? PluralCaseTable { get; set; }
        public bool UseSingular { get; set; }

        public NounTable()
        {
            UseSingular = true;
            SingularCaseTable = new CaseTable();
            PluralCaseTable = new CaseTable();
        }

        public NounTable(Noun originalNoun, CaseTable? singularCaseTable, CaseTable pluralCaseTable, bool useSingular)
        {
            OriginalNoun = originalNoun;
            SingularCaseTable = singularCaseTable;
            PluralCaseTable = pluralCaseTable;
            UseSingular = useSingular;
        }

        public override string ToString()
        {
            return $"Noun: {OriginalNoun}, Sing: {SingularCaseTable}, Pl: {PluralCaseTable}, UseSing: {UseSingular}";
        }
    }
}