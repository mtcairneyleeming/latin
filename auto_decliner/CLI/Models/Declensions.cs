using System;
using System.Collections.Generic;

namespace LatinAutoDeclineTester.Models
{
    public partial class Declensions
    {
        public Declensions()
        {
            Nouns = new HashSet<Nouns>();
        }

        public int DeclensionId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        public ICollection<Nouns> Nouns { get; set; }
    }
}
