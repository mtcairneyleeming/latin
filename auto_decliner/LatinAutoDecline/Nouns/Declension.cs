using System;

namespace LatinAutoDecline.Nouns
{
    class Declension
    {
        public Declension(EndingsTable masculine, EndingsTable feminine, EndingsTable neuter)
        {
            Masculine = masculine;
            Feminine = feminine;
            Neuter = neuter;
        }

        public EndingsTable Masculine { get; set; }
        public EndingsTable Feminine { get; set; }
        public EndingsTable Neuter { get; set; }

        public Cases GetCases(Gender gender, bool singular)
        {
            switch (gender)
            {
                case Gender.Feminine:
                    if (singular)
                        return Feminine.SingularCases;
                    return Feminine.PluralCases;
                case Gender.Masculine:
                    if (singular)
                        return Masculine.SingularCases;
                    return Masculine.PluralCases;
                case Gender.Neuter:
                    if (singular)
                        return Neuter.SingularCases;
                    return Neuter.PluralCases;
                default:
                    throw  new ArgumentException("The gender provided must be one of masculine, feminine or neuter");
            }
        }
    }
}