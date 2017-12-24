using System.Collections.Generic;

namespace decliner.Database
{
    public class Lemma
    {
        public Lemma()
        {
            Definitions = new HashSet<Definition>();
            Forms = new HashSet<Form>();
            SectionWords = new HashSet<SectionWord>();
            UserLearntWords = new HashSet<UserLearntWord>();
        }

        public int LemmaId { get; set; }
        public string LemmaText { get; set; }
        public string LemmaShortDef { get; set; }

        public LemmaData LemmaData { get; set; }
        public ICollection<Definition> Definitions { get; set; }
        public ICollection<Form> Forms { get; set; }
        public ICollection<SectionWord> SectionWords { get; set; }
        public ICollection<UserLearntWord> UserLearntWords { get; set; }
    }
}