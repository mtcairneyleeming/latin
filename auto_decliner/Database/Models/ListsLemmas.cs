﻿namespace LatinAutoDecline.Database.Models
{
    public partial class ListsLemmas
    {
        public int ListId { get; set; }
        public int LemmaId { get; set; }

        public Lemmas Lemma { get; set; }
        public Lists List { get; set; }
    }
}
