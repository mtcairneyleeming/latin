using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class LemmaData
    {
        public int LemmaId { get; set; }
        public int PartOfSpeech { get; set; }
        public int? CategoryId { get; set; }
        public int? GenderId { get; set; }
        public bool UseSingular { get; set; }

        public Category Category { get; set; }
        public Genders Gender { get; set; }
        public Lemmas Lemma { get; set; }
    }
}
