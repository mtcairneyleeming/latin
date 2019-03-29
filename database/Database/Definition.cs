namespace database.Database
{
    public class Definition
    {
        public int DefinitionId { get; set; }
        public int LemmaId { get; set; }
        public string Data { get; set; }
        public int Level { get; set; }
        public virtual Lemma Lemma { get; set; }
    }
}