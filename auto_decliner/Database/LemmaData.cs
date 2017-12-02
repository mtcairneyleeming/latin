namespace decliner.Database
{
    public class LemmaData
    {
        public int LemmaId { get; set; }
        public int PartOfSpeechId { get; set; }
        public int? CategoryId { get; set; }
        public int? GenderId { get; set; }
        public bool UseSingular { get; set; }

        public Category Category { get; set; }
        public Genders Gender { get; set; }
        public Lemma Lemma { get; set; }
        public PartOfSpeech PartOfSpeech { get; set; }
    }
}