namespace database.Database
{
    public class UserLearntWord
    {
        public int UserLearntWordId { get; set; }
        public int LemmaId { get; set; }
        public int RevisionStage { get; set; }

        public virtual Lemma Lemma { get; set; }
    }
}