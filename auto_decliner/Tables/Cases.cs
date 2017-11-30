using System;
using System.Collections.Generic;

namespace LatinAutoDecline.Tables
{
    public struct Cases
    {
        public readonly string Nominative;
        public readonly string Accusative;
        public readonly string Genitive;
        public readonly string Dative;
        public readonly string Ablative;
        public readonly string Vocative;

        public Cases(string nominative, string accusative, string genitive, string dative, string ablative, string vocative)
        {
            Nominative = nominative;
            Accusative = accusative;
            Genitive = genitive;
            Dative = dative;
            Ablative = ablative;
            Vocative = vocative;
        }

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