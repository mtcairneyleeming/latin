using System;

namespace LatinAutoDecline.Nouns
{
    /// <summary>
    /// A struct representing the needed components of a latin noun for this library
    /// Throws ArgumentException when a genitive singular form is not provided for 3rd declensionEnum nouns
    /// </summary>
    /// 
    public struct Noun
    {
        public Noun(string nominative, DeclensionEnum declensionEnum, Gender gender, Number number, string genitiveSingular) : this()
        {
            Nominative = nominative;
            Declension = declensionEnum;
            Gender = gender;
            Number = number;
            if (declensionEnum == DeclensionEnum.Three && string.IsNullOrEmpty(genitiveSingular))
            {
                throw new ArgumentException("A genitive singular form must be provided for 3rd declensionEnum nouns");
            }
            GenitiveSingular = genitiveSingular;
        }

        public string Nominative { get; set; }
        public DeclensionEnum Declension { get; set; }
        public Gender Gender { get; set; }
        public Number Number { get; set; }
        /// <summary>
        /// Unneeded unless in 3rd Declension
        /// </summary>
        public string GenitiveSingular { get; set; }
    }
}