namespace LatinAutoDecline.Database.Models
{
    public partial class Forms
    {
        public int Id { get; set; }
        public int LemmaId { get; set; }
        public string MorphCode { get; set; }
        public string Form { get; set; }
        public string MiscFeatures { get; set; }

        public Lemmas Lemma { get; set; }
    }
}
