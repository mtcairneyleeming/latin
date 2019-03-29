using System.Collections.Generic;

namespace database.Database
{
    public class Gender
    {
        public Gender()
        {
            LemmaData = new HashSet<LemmaData>();
        }

        public int GenderId { get; set; }
        public string GenderCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<LemmaData> LemmaData { get; set; }
    }
}