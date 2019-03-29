using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace database.Database
{
    public class PartOfSpeech
    {
        public PartOfSpeech()
        {
            LemmaData = new HashSet<LemmaData>();
        }

        [Key]
        public int PartId { get; set; }

        public string PartName { get; set; }
        public string PartDesc { get; set; }

        public virtual ICollection<LemmaData> LemmaData { get; set; }
    }
}