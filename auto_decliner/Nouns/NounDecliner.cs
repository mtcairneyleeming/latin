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
            _declensions = new Dictionary<Declension, DeclensionEndings>();
        }

        /// <summary>
        /// Load declension data
        /// </summary>
        public void Init()
        {
            var files = new Dictionary<string, Declension>()
            {
                {"LatinAutoDecline.Nouns.Resources.first.json", Declension.One},
                {"LatinAutoDecline.Nouns.Resources.second.json", Declension.Two},
                {"LatinAutoDecline.Nouns.Resources.second-r.json", Declension.TwoREnd},
                {"LatinAutoDecline.Nouns.Resources.third.json", Declension.Three},
                {"LatinAutoDecline.Nouns.Resources.third-i.json", Declension.ThreeIStem},
                {"LatinAutoDecline.Nouns.Resources.fourth.json", Declension.Four},
                {"LatinAutoDecline.Nouns.Resources.fifth.json", Declension.Five},
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
        public NounTable Decline(string nomSing)
        {
            throw new NotImplementedException(
                "Lookups to glean data from just the nom. sing. form have not been implemented yet");
            //return Decline(new Noun(nomSing, Declension.One, Gender.Feminine, PluralOnly.SingularAndPlural, ""));
        }

        public NounTable Decline(string nomSing, Declension declension, Gender gender, string genSing)
        {
            return Decline(new Noun(nomSing, declension, gender, false, genSing));
        }
        public NounTable Decline(string nomSing, Declension declension, Gender gender)
        {
            return Decline(new Noun(nomSing, declension, gender, false, ""));
        }

        public NounTable Decline(Noun inputNoun)
        {

            DeclensionEndings currentDeclensionEndings = _declensions[inputNoun.Declension];
            CaseTable singCaseTable = currentDeclensionEndings.GetCases(inputNoun.Gender, true);
            CaseTable plCaseTable = currentDeclensionEndings.GetCases(inputNoun.Gender, false);
            switch (inputNoun.Declension)
            {
                case Declension.One:
                    if (inputNoun.PluralOnly)
                    {
                        string stem = StripEnding(inputNoun.Nominative, "ae");
                        return new NounTable(inputNoun, null, AddEndings(plCaseTable, stem, null), false);
                    }
                    else
                    {
                        string stem = StripEnding(inputNoun.Nominative, "a");
                        return new NounTable(inputNoun, AddEndings(singCaseTable, stem, null),
                            AddEndings(plCaseTable, stem, null), true);
                    }
                case Declension.Two:
                case Declension.TwoREnd:
                    // TODO: consider exceptions
                    if (inputNoun.Nominative.EndsWith("r"))
                    {
                        // update declension to match odd stem & load correct case endings
                        inputNoun.Declension = Declension.TwoREnd;
                        singCaseTable = currentDeclensionEndings.GetCases(inputNoun.Gender, true);
                        plCaseTable = currentDeclensionEndings.GetCases(inputNoun.Gender, false);

                        string stem;
                        if (inputNoun.Nominative.EndsWith("ir"))
                        {
                            stem = StripEnding(inputNoun.Nominative, "ir");
                            return new NounTable(inputNoun, AddEndings(singCaseTable, stem, inputNoun.Nominative),
                                AddEndings(plCaseTable, stem, null), true);
                        }
                        else
                        {
                            stem = StripEnding(inputNoun.Nominative, "er");
                            return new NounTable(inputNoun, AddEndings(singCaseTable, stem, inputNoun.Nominative),
                                AddEndings(plCaseTable, stem, null), true);
                        }

                    }
                    else if (inputNoun.Gender == Gender.Masculine)
                    {
                        string stem = StripEnding(inputNoun.Nominative, "us");
                        return new NounTable(inputNoun, AddEndings(singCaseTable, stem, null),
                            AddEndings(plCaseTable, stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNoun.Nominative, "um");
                        return new NounTable(inputNoun, AddEndings(singCaseTable, stem, null),
                            AddEndings(plCaseTable, stem, null), true);
                    }
                case Declension.ThreeIStem:
                case Declension.Three:
                    // AARGH!
                    var result = IsIStem(inputNoun);
                    if (result.isIStem)
                    {
                        // update declension to match odd stem & load correct case endings
                        inputNoun.Declension = Declension.ThreeIStem;
                        singCaseTable = currentDeclensionEndings.GetCases(inputNoun.Gender, true);
                        plCaseTable = currentDeclensionEndings.GetCases(inputNoun.Gender, false);
                        return new NounTable(inputNoun, AddEndings(singCaseTable, result.stem, inputNoun.Nominative),
                            AddEndings(plCaseTable, result.stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNoun.GenitiveSingular, "is");
                        return new NounTable(inputNoun, AddEndings(singCaseTable, stem, inputNoun.Nominative),
                            AddEndings(plCaseTable, stem, null), true);
                    }

                case Declension.Four:
                    if (inputNoun.Gender == Gender.Masculine)
                    {
                        string stem = StripEnding(inputNoun.Nominative, "us");
                        return new NounTable(inputNoun, AddEndings(singCaseTable, stem, null),
                            AddEndings(plCaseTable, stem, null), true);
                    }
                    else
                    {
                        string stem = StripEnding(inputNoun.Nominative, "u");
                        return new NounTable(inputNoun, AddEndings(singCaseTable, stem, null),
                            AddEndings(plCaseTable, stem, null), true);
                    }
                case Declension.Five:
                    {
                        string stem = StripEnding(inputNoun.Nominative, "es");
                        return new NounTable(inputNoun, AddEndings(singCaseTable, stem, null), AddEndings(plCaseTable, stem, null),
                            true);
                    }
                case Declension.Irregular:
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
        private CaseTable AddEndings(CaseTable endings, string stem, string nominative)
        {
            // NB: uses provided values if they are provided, to avoid odd behaviour, e.g. for 3rd decl i - stem nominative forms.
            // Replace dashes with nominative form where the nominative form is complex and other forms follow it
            var nom = (string.IsNullOrEmpty(nominative) ? (stem + endings.Nominative) : nominative);
            var acc = (endings.Accusative == "-") ? nom : stem + endings.Accusative;
            var gen = (endings.Genitive == "-") ? nom : stem + endings.Genitive;
            var dat = (endings.Dative == "-") ? nom : stem + endings.Accusative;
            var abl = (endings.Ablative == "-") ? nom : stem + endings.Ablative;
            var voc = (endings.Vocative == "-") ? nom : stem + endings.Vocative;

            return new CaseTable(nom, acc, gen, dat, abl, voc);
        }


    }
}
