using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global

namespace database.Database
{
    public class Lemma
    {
        public Lemma()
        {
            Definitions = new HashSet<Definition>();
            Forms = new HashSet<Form>();
        }

        public int LemmaId { get; set; }
        public string LemmaText { get; set; }
        public string LemmaShortDef { get; set; }

        public virtual LemmaData LemmaData { get; set; }
        public virtual ICollection<Definition> Definitions { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
        public virtual UserLearntWord UserLearntWord { get; set; }
    }
}