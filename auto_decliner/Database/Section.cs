using System.Collections.Generic;

namespace decliner.Database
{
    public class Section
    {
        public Section()
        {
            SectionWords = new HashSet<SectionWord>();
        }

        public string Name { get; set; }
        public int SectionId { get; set; }
        public int ListId { get; set; }

        public List List { get; set; }
        public ICollection<SectionWord> SectionWords { get; set; }
    }
}