using System.Collections.Generic;

namespace decliner.Database
{
    public class Category
    {
        public Category()
        {
            LemmaData = new HashSet<LemmaData>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string CategoryIdentifier { get; set; }

        public ICollection<LemmaData> LemmaData { get; set; }
    }
}