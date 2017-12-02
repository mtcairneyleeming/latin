using System.Collections.Generic;

namespace decliner.Database
{
    public class Genders
    {
        public Genders()
        {
            LemmaData = new HashSet<LemmaData>();
        }

        public int GenderId { get; set; }
        public string GenderCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<LemmaData> LemmaData { get; set; }
    }
}