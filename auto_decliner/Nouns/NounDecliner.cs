using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LatinAutoDecline.Tables;
using Newtonsoft.Json;

// ReSharper disable HeuristicUnreachableCode

#pragma warning disable 162

namespace LatinAutoDecline.Nouns
{
    public class NounDecliner
    {
        public NounDecliner()
        {
            _declensions = new Dictionary<Declension, DeclensionEndings>();
        }

        /// <summary>
        /// Load declension data
        /// </summary>
        public void Init()
        {
            var files = new Dictionary<string, Declension>
            {
                {"LatinAutoDecline.Nouns.Resources.first.json", Declension.One},
                {"LatinAutoDecline.Nouns.Resources.second.json", Declension.Two},
                {"LatinAutoDecline.Nouns.Resources.second-r.json", Declension.TwoREnd},
                {"LatinAutoDecline.Nouns.Resources.third.json", Declension.Three},
                {"LatinAutoDecline.Nouns.Resources.third-i.json", Declension.ThreeIStem},
                {"LatinAutoDecline.Nouns.Resources.fourth.json", Declension.Four},
                {"LatinAutoDecline.Nouns.Resources.fifth.json", Declension.Five}
            };

            var assembly = typeof(NounDecliner).GetTypeInfo().Assembly;

            foreach (KeyValuePair<string, Declension> pair in files)
            {
                using (Stream resource = assembly.GetManifestResourceStream(pair.Key))
                {
                    using (StreamReader sr = new StreamReader(resource))
                    {
                        string data = sr.ReadToEnd();
                        var declension = JsonConvert.DeserializeObject<DeclensionEndings>(data);
                        _declensions[pair.Value] = declension;
                    }
                }

            }


        }


        LatinSyllablifier Syllablifier = new LatinSyllablifier();
        private Dictionary<Declension, DeclensionEndings> _declensions;
        public Tables.Noun Decline(string nomSing)
        {
            throw new NotImplementedException(
                "Lookups to glean data from just the nom. sing. form have not been implemented yet");
            //return Decline(new NounData(nomSing, Declension.One, Gender.Feminine, PluralOnly.SingularAndPlural, ""));
        }

        public Tables.Noun Decline(string nomSing, Declension declension, Gender gender, string genSing)
        {
            return Decline(new NounData(nomSing, declension, gender, false, genSing));
        }
        public Tables.Noun Decline(string nomSing, Declension declension, Gender gender)
        {
            return Decline(new NounData(nomSing, declension, gender, false, ""));
        }

        public Tables.Noun Decline(NounData inputNounData)
        {

            DeclensionEndings currentDeclensionEndings = _declensions[inputNounData.Declension];
            Cases singCases = currentDeclensionEndings.GetCases(inputNounData.Gender, true);
            Cases plCases = currentDeclensionEndings.GetCases(inputNounData.Gender, false);
            switch (inputNounData.Declension)
            {
                case Declension.One:
                    if (inputNounData.PluralOnly)
                    {
                        string stem = StripEnding(inputNounData.Nominative, "ae");
                        return new Tables.Noun(inputNounData, null, AddEndings(plCases, stem, null), false);
                    }
                    else
                    {
                        string stem = StripEnding(inputNounData.Nominative, "a");
                        return new Tables.Noun(inputNounData, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                case Declension.Two:
                case Declension.TwoREnd:
                    // TODO: consider exceptions
                    if (inputNounData.Nominative.EndsWith("r"))
                    {
                        // update declension to match odd stem & load correct case endings
                        inputNounData.Declension = Declension.TwoREnd;
                        singCases = currentDeclensionEndings.GetCases(inputNounData.Gender, true);
                        plCases = currentDeclensionEndings.GetCases(inputNounData.Gender, false);

                        string stem;
                        if (inputNounData.Nominative.EndsWith("ir"))
                        {
                            stem = StripEnding(inputNounData.Nominative, "ir");
                            return new Tables.Noun(inputNounData, AddEndings(singCases, stem, inputNounData.Nominative),
                                AddEndings(plCases, stem, null), true);
                        }
                        stem = StripEnding(inputNounData.Nominative, "er");
                        return new Tables.Noun(inputNounData, AddEndings(singCases, stem, inputNounData.Nominative),
                            AddEndings(plCases, stem, null), true);
                    }
                    else if (inputNounData.Gender == Gender.Masculine)
                    {
                        string stem = StripEnding(inputNounData.Nominative, "us");
                        return new Tables.Noun(inputNounData, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNounData.Nominative, "um");
                        return new Tables.Noun(inputNounData, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                case Declension.ThreeIStem:
                case Declension.Three:
                    // AARGH!
                    var result = IsIStem(inputNounData);
                    if (result.isIStem)
                    {
                        // update declension to match odd stem & load correct case endings
                        inputNounData.Declension = Declension.ThreeIStem;
                        singCases = currentDeclensionEndings.GetCases(inputNounData.Gender, true);
                        plCases = currentDeclensionEndings.GetCases(inputNounData.Gender, false);
                        return new Tables.Noun(inputNounData, AddEndings(singCases, result.stem, inputNounData.Nominative),
                            AddEndings(plCases, result.stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNounData.GenitiveSingular, "is");
                        return new Tables.Noun(inputNounData, AddEndings(singCases, stem, inputNounData.Nominative),
                            AddEndings(plCases, stem, null), true);
                    }

                case Declension.Four:
                    if (inputNounData.Gender == Gender.Masculine)
                    {
                        string stem = StripEnding(inputNounData.Nominative, "us");
                        return new Tables.Noun(inputNounData, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNounData.Nominative, "u");
                        return new Tables.Noun(inputNounData, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                case Declension.Five:
                    {
                        string stem = StripEnding(inputNounData.Nominative, "es");
                        return new Tables.Noun(inputNounData, AddEndings(singCases, stem, null), AddEndings(plCases, stem, null),
                            true);
                    }
                case Declension.Irregular:
                    throw new NotImplementedException("Entirely irregular nouns aren't supported yet");
                    return new Tables.Noun();
                default:
                    throw new ArgumentException("The noun provided must be of a valid declension", nameof(inputNounData.Declension));
            }
        }
        private (bool isIStem, string stem) IsIStem(NounData input)
        {
            // check if this is an i-stem 

            // split syllables
            var nom = Syllablifier.SyllabifyWord(input.Nominative);
            var gen = Syllablifier.SyllabifyWord(input.GenitiveSingular);
            if (nom.Count == gen.Count)
            {
                return (true, StripEnding(input.GenitiveSingular, "is"));
            }
            // nom. form is only 1 syllable long
            if (nom.Count == 1)
            {
                // nom ends in -x/-s
                if (input.Nominative.EndsWith("x") || input.Nominative.EndsWith("s"))
                {
                    // check that there are 2 consonants before the ending
                    string stem = StripEnding(input.Nominative, "is");
                    string consonants = "qwrtypsdfghjklzxcvbnm";

                    if (consonants.Contains(stem.Last()) && consonants.Contains(stem[stem.Length - 2].ToString()))
                    {
                        return (true, StripEnding(input.GenitiveSingular, "is"));
                    }
                }
            }
            // neuter + endings
            if (input.Gender == Gender.Neuter)
            {
                if (input.Nominative.EndsWith("e") || input.Nominative.EndsWith("al") || input.Nominative.EndsWith("ar"))
                {
                    return (true, StripEnding(input.GenitiveSingular, "is"));
                }
            }

            // it isnt
            return (false, "");
        }
        private string StripEnding(string initialForm, string ending)
        {
            return initialForm.Substring(0, initialForm.LastIndexOf(ending, StringComparison.Ordinal));
        }
        //private string AutoStripEnding(Cases endings, string nomSing, string genSing = "")
        //{
        //    if (string.IsNullOrEmpty(genSing))
        //    {
        //        if (nomSing.EndsWith(endings.Nominative))
        //        {
        //            return StripEnding(nomSing, endings.Nominative);
        //        }
        //        throw new ArgumentException("The nom. sing. form provided does not end with the ending provided", nameof(nomSing));
        //    }
        //    else
        //    {
        //        if (genSing.EndsWith(endings.Genitive))
        //        {
        //            return StripEnding(genSing, endings.Genitive);
        //        }
        //        else
        //        {
        //            if (nomSing.EndsWith(endings.Nominative))
        //            {
        //                return StripEnding(nomSing, endings.Nominative);
        //            }
        //            throw new ArgumentException(
        //                    "The gen. sing. form provided does not end with the ending provided", nameof(genSing),
        //                    new ArgumentException("The nom. sing form provided also did not end with the ending provided", nameof(nomSing)));
        //        }

        //    }
        //}
        private Cases AddEndings(Cases endings, string stem, string nominative)
        {
            // NB: uses provided values if they are provided, to avoid odd behaviour, e.g. for 3rd decl i - stem nominative forms.
            // Replace dashes with nominative form where the nominative form is complex and other forms follow it
            var nom = (string.IsNullOrEmpty(nominative) ? (stem + endings.Nominative) : nominative);
            var acc = (endings.Accusative == "-") ? nom : stem + endings.Accusative;
            var gen = (endings.Genitive == "-") ? nom : stem + endings.Genitive;
            var dat = (endings.Dative == "-") ? nom : stem + endings.Accusative;
            var abl = (endings.Ablative == "-") ? nom : stem + endings.Ablative;
            var voc = (endings.Vocative == "-") ? nom : stem + endings.Vocative;

            return new Cases(nom, acc, gen, dat, abl, voc);
        }


    }
}
