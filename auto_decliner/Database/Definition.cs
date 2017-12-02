namespace decliner.Database
{
    public class Definition
    {
        public int DefinitionId { get; set; }
        public int LemmaId { get; set; }
        public string Data { get; set; }
        public int Level { get; set; }
        public Lemma Lemma { get; set; }
    }

    public enum DefinitionLevel
    {
        KS3 = 0,
        GCSE = 1,
        ALevel = 2,
        Dictionary = 3
    }
}