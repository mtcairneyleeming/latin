using Newtonsoft.Json;

namespace database.Database
{
    public class Form
    {
        public int FormId { get; set; }
        public int LemmaId { get; set; }
        public string MorphCode { get; set; }

        [JsonProperty(PropertyName = "Form")]
        public string Text { get; set; }

        public string MiscFeatures { get; set; }

        public virtual Lemma Lemma { get; set; }
    }
}