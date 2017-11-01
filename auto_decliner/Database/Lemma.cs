using System.Collections.Generic;

namespace LatinAutoDecline.Database
{
    public class Lemma
    {
        public Lemma()
        {
            Definition = new HashSet<Definition>();
            Forms = new HashSet<Form>();
            SectionWords = new HashSet<SectionWord>();
            UserLearntWords = new HashSet<UserLearntWord>();
        }

        public int LemmaId { get; set; }
        public string LemmaText { get; set; }
        public string LemmaShortDef { get; set; }

        public LemmaData LemmaData { get; set; }
        public ICollection<Definition> Definition { get; set; }
        public ICollection<Form> Forms { get; set; }
        public ICollection<SectionWord> SectionWords { get; set; }
        public ICollection<UserLearntWord> UserLearntWords { get; set; }
    }
}
