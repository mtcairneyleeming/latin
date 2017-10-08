using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class Nouns
    {
        public int NounId { get; set; }
        public int LemmaId { get; set; }
        public int DeclensionId { get; set; }
        public int? GenderId { get; set; }
        public bool UseSingular { get; set; }

        public Declensions Declension { get; set; }
        public Genders Gender { get; set; }
        public Lemmas Lemma { get; set; }
    }
}
