namespace LatinAutoDecline.Database
{
    public class SectionWord
    {
        public int SectionId { get; set; }
        public int LemmaId { get; set; }

        public Lemma Lemma { get; set; }
        public Section Section { get; set; }
    }
}
