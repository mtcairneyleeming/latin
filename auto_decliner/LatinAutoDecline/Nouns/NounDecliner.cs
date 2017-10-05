using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;
// ReSharper disable HeuristicUnreachableCode

#pragma warning disable 162

namespace LatinAutoDecline.Nouns
{
    public class NounDecliner
    {
        public NounDecliner()
        {
            _declensions = new Dictionary<DeclensionEnum, Declension>();
        }

        /// <summary>
        /// Load declension data
        /// </summary>
        public void Init()
        {
            var files = new Dictionary<string, DeclensionEnum>()
            {
                {"LatinAutoDecline.Nouns.Resources.first.json", DeclensionEnum.One},
                {"LatinAutoDecline.Nouns.Resources.second.json", DeclensionEnum.Two},
                {"LatinAutoDecline.Nouns.Resources.second-r.json", DeclensionEnum.TwoREnd},
                {"LatinAutoDecline.Nouns.Resources.third.json", DeclensionEnum.Three},
                {"LatinAutoDecline.Nouns.Resources.third-i.json", DeclensionEnum.ThreeIStem},
                {"LatinAutoDecline.Nouns.Resources.fourth.json", DeclensionEnum.Four},
                {"LatinAutoDecline.Nouns.Resources.fifth.json", DeclensionEnum.Five},
            };

            var assembly = typeof(NounDecliner).GetTypeInfo().Assembly;

            foreach (KeyValuePair<string, DeclensionEnum> pair in files)
            {
                using (Stream resource = assembly.GetManifestResourceStream(pair.Key))
                {
                    using (StreamReader sr = new StreamReader(resource))
                    {
                        string data = sr.ReadToEnd();
                        var declension = JsonConvert.DeserializeObject<Declension>(data);
                        _declensions[pair.Value] = declension;
                    }
                }

            }


        }


        LatinSyllablifier Syllablifier = new LatinSyllablifier();
        private Dictionary<DeclensionEnum, Declension> _declensions;
        public NounTable Decline(string nomSing)
        {
            throw new NotImplementedException(
                "Lookups to glean data from just the nom. sing. form have not been implemented yet");
            //return Decline(new Noun(nomSing, Declension.One, Gender.Feminine, Number.SingularAndPlural, ""));
        }

        public NounTable Decline(string nomSing, DeclensionEnum declensionEnum, Gender gender, Number number, string genSing)
        {
            return Decline(new Noun(nomSing, declensionEnum, gender, number, genSing));
        }
        public NounTable Decline(string nomSing, DeclensionEnum declensionEnum, Gender gender, Number number)
        {
            return Decline(new Noun(nomSing, declensionEnum, gender, number, ""));
        }

        public NounTable Decline(Noun inputNoun)
        {

            Declension currentDeclension = _declensions[inputNoun.Declension];
            Cases singCases = currentDeclension.GetCases(inputNoun.Gender, true);
            Cases plCases = currentDeclension.GetCases(inputNoun.Gender, false);
            switch (inputNoun.Declension)
            {
                case DeclensionEnum.One:
                    if (inputNoun.Number == Number.PluralOnly)
                    {
                        string stem = StripEnding(inputNoun.Nominative, "ae");
                        return new NounTable(inputNoun, null, AddEndings(plCases, stem, null), false);
                    }
                    else
                    {
                        string stem = StripEnding(inputNoun.Nominative, "a");
                        return new NounTable(inputNoun, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                case DeclensionEnum.Two:
                case DeclensionEnum.TwoREnd:
                    // TODO: consider exceptions
                    if (inputNoun.Nominative.EndsWith("r"))
                    {
                        // update declension to match odd stem & load correct case endings
                        inputNoun.Declension = DeclensionEnum.TwoREnd;
                        singCases = currentDeclension.GetCases(inputNoun.Gender, true);
                        plCases = currentDeclension.GetCases(inputNoun.Gender, false);

                        string stem;
                        if (inputNoun.Nominative.EndsWith("ir"))
                        {
                            stem = StripEnding(inputNoun.Nominative, "ir");
                            return new NounTable(inputNoun, AddEndings(singCases, stem, inputNoun.Nominative),
                                AddEndings(plCases, stem, null), true);
                        }
                        else
                        {
                            stem = StripEnding(inputNoun.Nominative, "er");
                            return new NounTable(inputNoun, AddEndings(singCases, stem, inputNoun.Nominative),
                                AddEndings(plCases, stem, null), true);
                        }

                    }
                    else if (inputNoun.Gender == Gender.Masculine)
                    {
                        string stem = StripEnding(inputNoun.Nominative, "us");
                        return new NounTable(inputNoun, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNoun.Nominative, "um");
                        return new NounTable(inputNoun, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                case DeclensionEnum.ThreeIStem:
                case DeclensionEnum.Three:
                    // AARGH!
                    var result = IsIStem(inputNoun);
                    if (result.isIStem)
                    {
                        // update declension to match odd stem & load correct case endings
                        inputNoun.Declension = DeclensionEnum.ThreeIStem;
                        singCases = currentDeclension.GetCases(inputNoun.Gender, true);
                        plCases = currentDeclension.GetCases(inputNoun.Gender, false);
                        return new NounTable(inputNoun, AddEndings(singCases, result.stem, inputNoun.Nominative),
                            AddEndings(plCases, result.stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNoun.GenitiveSingular, "is");
                        return new NounTable(inputNoun, AddEndings(singCases, stem, inputNoun.Nominative),
                            AddEndings(plCases, stem, null), true);
                    }

                case DeclensionEnum.Four:
                    if (inputNoun.Gender == Gender.Masculine)
                    {
                        string stem = StripEnding(inputNoun.Nominative, "us");
                        return new NounTable(inputNoun, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNoun.Nominative, "u");
                        return new NounTable(inputNoun, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                case DeclensionEnum.Five:
                    {
                        string stem = StripEnding(inputNoun.Nominative, "es");
                        return new NounTable(inputNoun, AddEndings(singCases, stem, null), AddEndings(plCases, stem, null),
                            true);
                    }
                case DeclensionEnum.Irregular:
                    throw new NotImplementedException("Entirely irregular nouns aren't supported yet");
                    return new NounTable();
                default:
                    throw new ArgumentException("The noun provided must be of a valid declension", nameof(inputNoun.Declension));
            }
        }
        private (bool isIStem, string stem) IsIStem(Noun input)
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
