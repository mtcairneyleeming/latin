using System;

namespace LatinAutoDecline.Nouns
{
    /// <summary>
    /// A struct representing the needed components of a latin noun for this library
    /// Throws ArgumentException when a genitive singular form is not provided for 3rd declensionEnum nouns
    /// </summary>
    /// 
    public struct NounData
    {
        public NounData(string nominative, Declension declension, Gender gender, bool pluralOnly, string genitiveSingular) : this()
        {
            Nominative = nominative;
            Declension = declension;
            Gender = gender;
            PluralOnly = pluralOnly;
            if (declension == Declension.Three && string.IsNullOrEmpty(genitiveSingular))
            {
                throw new ArgumentException("A genitive singular form must be provided for 3rd declensionEnum nouns");
            }
            GenitiveSingular = genitiveSingular;
        }

        public string Nominative { get; set; }
        public Declension Declension { get; set; }
        public Gender Gender { get; set; }
        public bool PluralOnly { get; set; }
        /// <summary>
        /// Unneeded unless in 3rd Declension
        /// </summary>
        public string GenitiveSingular { get; set; }
    }
}