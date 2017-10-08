using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class Genders
    {
        public Genders()
        {
            Nouns = new HashSet<Nouns>();
        }

        public int GenderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Nouns> Nouns { get; set; }
    }
}
