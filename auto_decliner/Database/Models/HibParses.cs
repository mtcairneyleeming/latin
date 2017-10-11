namespace LatinAutoDecline.Database.Models
{
    public partial class HibParses
    {
        public int Id { get; set; }
        public int LemmaId { get; set; }
        public string MorphCode { get; set; }
        public string ExpandedForm { get; set; }
        public string Form { get; set; }
        public string BareForm { get; set; }
        public string Dialects { get; set; }
        public string MiscFeatures { get; set; }
    }
}
