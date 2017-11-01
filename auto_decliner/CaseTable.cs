using System;
using System.Collections.Generic;

namespace LatinAutoDecline
{
    public struct CaseTable : IEquatable<CaseTable>
    {
        public string Nominative;
        public string Accusative;
        public string Genitive;
        public string Dative;
        public string Ablative;
        public string Vocative;

        public CaseTable(string nominative, string accusative, string genitive, string dative, string ablative, string vocative)
        {
            Nominative = nominative;
            Accusative = accusative;
            Genitive = genitive;
            Dative = dative;
            Ablative = ablative;
            Vocative = vocative;
        }

        public override bool Equals(object obj)
        {
            return obj is CaseTable cases && Equals(cases);
        }

        public bool Equals(CaseTable other)
        {
            return Nominative == other.Nominative &&
                   Accusative == other.Accusative &&
                   Genitive == other.Genitive &&
                   Dative == other.Dative &&
                   Ablative == other.Ablative &&
                   Vocative == other.Vocative;
        }

        public override int GetHashCode()
        {
            var hashCode = -937184933;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nominative);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Accusative);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Genitive);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Dative);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Ablative);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Vocative);
            return hashCode;
        }

        public override string ToString()
        {
            return
                $"Nom: {Nominative}, Acc: {Accusative}, Gen: {Genitive}, Dat: {Dative}, Abl: {Ablative}, Voc: {Vocative}";
        }

        public static bool operator ==(CaseTable cases1, CaseTable cases2)
        {
            return cases1.Equals(cases2);
        }

        public static bool operator !=(CaseTable cases1, CaseTable cases2)
        {
            return !(cases1 == cases2);
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