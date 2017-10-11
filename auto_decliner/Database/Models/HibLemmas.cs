namespace LatinAutoDecline.Database.Models
{
    public partial class HibLemmas
    {
        public int LemmaId { get; set; }
        public string LemmaText { get; set; }
        public string BareHeadword { get; set; }
        public int? LemmaSequenceNumber { get; set; }
        public string LemmaShortDef { get; set; }
    }
}
