using Newtonsoft.Json;

namespace LatinAutoDecline.Database
{
    public class Form
    {
        public int Id { get; set; }
        public int LemmaId { get; set; }
        public string MorphCode { get; set; }
        [JsonProperty(PropertyName = "Form")]
        public string Text { get; set; }
        public string MiscFeatures { get; set; }

        public Lemma Lemma { get; set; }
    }
}
