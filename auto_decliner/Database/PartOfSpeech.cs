using System.Collections.Generic;

namespace LatinAutoDecline.Database
{
    public class PartOfSpeech
    {
        public PartOfSpeech()
        {
            LemmaData = new HashSet<LemmaData>();
        }

        public int PartId { get; set; }
        public string PartName { get; set; }
        public string PartDesc { get; set; }

        public ICollection<LemmaData> LemmaData { get; set; }
    }
}
