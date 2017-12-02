using System;
using decliner.Tables;

namespace decliner.Nouns
{
    internal class DeclensionEndings
    {
        public DeclensionEndings(NounPluralities masculine, NounPluralities feminine, NounPluralities neuter)
        {
            Masculine = masculine;
            Feminine = feminine;
            Neuter = neuter;
        }

        public DeclensionEndings()
        {
            Masculine = new NounPluralities();
            Feminine  = new NounPluralities();
            Neuter = new NounPluralities();
        }

        public NounPluralities Masculine { get; set; }
        public NounPluralities Feminine { get; set; }
        public NounPluralities Neuter { get; set; }

        public Cases GetCases(Gender gender, bool singular)
        {
            switch (gender)
            {
                case Gender.Feminine:
                    if (singular)
                        return Feminine.Singular;
                    return Feminine.Plural;
                case Gender.Masculine:
                    if (singular)
                        return Masculine.Singular;
                    return Masculine.Plural;
                case Gender.Neuter:
                    if (singular)
                        return Neuter.Singular;
                    return Neuter.Plural;
                default:
                    throw new ArgumentException("The gender provided must be one of masculine, feminine or neuter");
            }
        }
    }
}