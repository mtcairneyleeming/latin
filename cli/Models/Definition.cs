using System;
using System.Collections.Generic;

namespace LatinAutoDeclineTester.Models
{
    public partial class Definition
    {
        public int DefinitionId { get; set; }
        public int LemmaId { get; set; }
        public string Alevel { get; set; }

        public Lemmas Lemma { get; set; }
    }
}
