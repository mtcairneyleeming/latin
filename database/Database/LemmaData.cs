using System.ComponentModel.DataAnnotations;

namespace database.Database
{
    public class LemmaData
    {
        [Key]
        public int LemmaId { get; set; }

        public int? PartOfSpeechId { get; set; }
        public int? CategoryId { get; set; }
        public int? GenderId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Lemma Lemma { get; set; }
        public virtual PartOfSpeech PartOfSpeech { get; set; }
    }
}