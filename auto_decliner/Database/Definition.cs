namespace LatinAutoDecline.Database
{
    public class Definition
    {
        public int DefinitionId { get; set; }
        public int LemmaId { get; set; }
        public string Alevel { get; set; }

        public Lemma Lemma { get; set; }
    }
}
