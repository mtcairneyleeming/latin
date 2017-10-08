using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class UserLearntWords
    {
        public int UserId { get; set; }
        public int LemmaId { get; set; }

        public Lemmas Lemma { get; set; }
        public Users User { get; set; }
    }
}
