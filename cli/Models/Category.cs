using System;
using System.Collections.Generic;

namespace LatinAutoDeclineTester.Models
{
    public partial class Category
    {
        public Category()
        {
            LemmaData = new HashSet<LemmaData>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        public ICollection<LemmaData> LemmaData { get; set; }
    }
}
