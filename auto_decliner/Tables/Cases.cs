namespace decliner.Tables
{
    public class Cases
    {
        public Cases()
        {
        }

        public Cases(string nominative, string accusative, string genitive, string dative, string ablative,
            string vocative)
        {
            Nominative = nominative;
            Accusative = accusative;
            Genitive = genitive;
            Dative = dative;
            Ablative = ablative;
            Vocative = vocative;
        }

        public string Nominative { get; set; }
        public string Accusative { get; set; }
        public string Genitive { get; set; }
        public string Dative { get; set; }
        public string Ablative { get; set; }
        public string Vocative { get; set; }

        public override string ToString()
        {
            return
                $"Nom: {Nominative}, Acc: {Accusative}, Gen: {Genitive}, Dat: {Dative}, Abl: {Ablative}, Voc: {Vocative}";
        }

        // get the form for a case
        public string GetForm(Case cas)
        {
            switch (cas)
            {
                case Case.Nominative:
                    return Nominative;
                case Case.Accusative:
                    return Accusative;
                case Case.Genitive:
                    return Genitive;
                case Case.Dative:
                    return Dative;
                case Case.Ablative:
                    return Ablative;
                case Case.Vocative:
                    return Vocative;
                default:
                    return "";
            }
        }
    }
}