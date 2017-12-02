using System;

namespace decliner.Database
{
    public class UserLearntWord
    {
        public string UserId { get; set; }
        public int LemmaId { get; set; }
        public DateTime NextRevision { get; set; }
        public int RevisionStage { get; set; }

        public Lemma Lemma { get; set; }
    }
}