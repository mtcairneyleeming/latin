using System;
using System.Collections.Generic;

namespace LatinAutoDeclineTester.Models
{
    public partial class Lemmas
    {
        public Lemmas()
        {
            Definition = new HashSet<Definition>();
            Forms = new HashSet<Forms>();
            ListsLemmas = new HashSet<ListsLemmas>();
            Nouns = new HashSet<Nouns>();
            UserLearntWords = new HashSet<UserLearntWords>();
        }

        public int LemmaId { get; set; }
        public string LemmaText { get; set; }
        public string LemmaShortDef { get; set; }

        public ICollection<Definition> Definition { get; set; }
        public ICollection<Forms> Forms { get; set; }
        public ICollection<ListsLemmas> ListsLemmas { get; set; }
        public ICollection<Nouns> Nouns { get; set; }
        public ICollection<UserLearntWords> UserLearntWords { get; set; }
    }
}
