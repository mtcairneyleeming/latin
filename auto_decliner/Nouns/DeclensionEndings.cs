using System;

namespace LatinAutoDecline.Nouns
{
    class DeclensionEndings
    {
        public DeclensionEndings(EndingsTable masculine, EndingsTable feminine, EndingsTable neuter)
        {
            Masculine = masculine;
            Feminine = feminine;
            Neuter = neuter;
        }

        public EndingsTable Masculine { get; set; }
        public EndingsTable Feminine { get; set; }
        public EndingsTable Neuter { get; set; }

        public CaseTable GetCases(Gender gender, bool singular)
        {
            switch (gender)
            {
                case Gender.Feminine:
                    if (singular)
                        return Feminine.SingularCaseTable;
                    return Feminine.PluralCaseTable;
                case Gender.Masculine:
                    if (singular)
                        return Masculine.SingularCaseTable;
                    return Masculine.PluralCaseTable;
                case Gender.Neuter:
                    if (singular)
                        return Neuter.SingularCaseTable;
                    return Neuter.PluralCaseTable;
                default:
                    throw  new ArgumentException("The gender provided must be one of masculine, feminine or neuter");
            }
        }
    }
}