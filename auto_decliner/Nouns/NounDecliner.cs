using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using decliner.Tables;
using Newtonsoft.Json;

// ReSharper disable HeuristicUnreachableCode

#pragma warning disable 162

namespace decliner.Nouns
{
    public class NounDecliner
    {
        private readonly Dictionary<Declension, DeclensionEndings> _declensions;


        private readonly LatinSyllablifier Syllablifier = new LatinSyllablifier();

        public NounDecliner()
        {
            _declensions = new Dictionary<Declension, DeclensionEndings>();
        }

        /// <summary>
        ///     Load declension data
        /// </summary>
        public void Init()
        {
            var files = new Dictionary<string, Declension>
            {
                {"decliner.Nouns.Resources.first.json", Declension.One},
                {"decliner.Nouns.Resources.second.json", Declension.Two},
                {"decliner.Nouns.Resources.second-r.json", Declension.TwoREnd},
                {"decliner.Nouns.Resources.third.json", Declension.Three},
                {"decliner.Nouns.Resources.third-i.json", Declension.ThreeIStem},
                {"decliner.Nouns.Resources.fourth.json", Declension.Four},
                {"decliner.Nouns.Resources.fifth.json", Declension.Five}
            };

            var assembly = typeof(NounDecliner).GetTypeInfo().Assembly;

            foreach (var pair in files)
                using (var resource = assembly.GetManifestResourceStream(pair.Key))
                {
                    using (var sr = new StreamReader(resource))
                    {
                        var data = sr.ReadToEnd();
                        var declension = JsonConvert.DeserializeObject<DeclensionEndings>(data);
                        _declensions[pair.Value] = declension;
                    }
                }
        }

        public Noun Decline(string nomSing)
        {
            throw new NotImplementedException(
                "Lookups to glean data from just the nom. sing. form have not been implemented yet");
            //return Decline(new NounData(nomSing, Declension.One, Gender.Feminine, PluralOnly.SingularAndPlural, ""));
        }

        public Noun Decline(string nomSing, Declension declension, Gender gender, string genSing)
        {
            return Decline(new NounData(nomSing, declension, gender, false, genSing));
        }

        public Noun Decline(string nomSing, Declension declension, Gender gender)
        {
            return Decline(new NounData(nomSing, declension, gender, false, ""));
        }

        public Noun Decline(NounData inputNounData)
        {
            var currentDeclensionEndings = _declensions[inputNounData.Declension];
            var singCases = currentDeclensionEndings.GetCases(inputNounData.Gender, true);
            var plCases = currentDeclensionEndings.GetCases(inputNounData.Gender, false);
            switch (inputNounData.Declension)
            {
                case Declension.One:
                    if (inputNounData.PluralOnly)
                    {
                        var stem = StripEnding(inputNounData.Nominative, "ae");
                        return new Noun(inputNounData, null, AddEndings(plCases, stem, null), false);
                    }
                    else
                    {
                        var stem = StripEnding(inputNounData.Nominative, "a");
                        return new Noun(inputNounData, AddEndings(singCases, stem, null),
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
                            return new Noun(inputNounData, AddEndings(singCases, stem, inputNounData.Nominative),
                                AddEndings(plCases, stem, null), true);
                        }

                        stem = StripEnding(inputNounData.Nominative, "er");
                        return new Noun(inputNounData, AddEndings(singCases, stem, inputNounData.Nominative),
                            AddEndings(plCases, stem, null), true);
                    }
                    else if (inputNounData.Gender == Gender.Masculine)
                    {
                        var stem = StripEnding(inputNounData.Nominative, "us");
                        return new Noun(inputNounData, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                    else
                    {
                        var stem = StripEnding(inputNounData.Nominative, "um");
                        return new Noun(inputNounData, AddEndings(singCases, stem, null),
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
                        return new Noun(inputNounData, AddEndings(singCases, result.stem, inputNounData.Nominative),
                            AddEndings(plCases, result.stem, null), true);
                    }
                    else
                    {
                        var stem = StripEnding(inputNounData.GenitiveSingular, "is");
                        return new Noun(inputNounData, AddEndings(singCases, stem, inputNounData.Nominative),
                            AddEndings(plCases, stem, null), true);
                    }

                case Declension.Four:
                    if (inputNounData.Gender == Gender.Masculine)
                    {
                        var stem = StripEnding(inputNounData.Nominative, "us");
                        return new Noun(inputNounData, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                    else
                    {
                        var stem = StripEnding(inputNounData.Nominative, "u");
                        return new Noun(inputNounData, AddEndings(singCases, stem, null),
                            AddEndings(plCases, stem, null), true);
                    }
                case Declension.Five:
                {
                    var stem = StripEnding(inputNounData.Nominative, "es");
                    return new Noun(inputNounData, AddEndings(singCases, stem, null), AddEndings(plCases, stem, null),
                        true);
                }
                case Declension.Irregular:
                    throw new NotImplementedException("Entirely irregular nouns aren't supported yet");
                    return new Noun();
                default:
                    throw new ArgumentException("The noun provided must be of a valid declension",
                        nameof(inputNounData.Declension));
            }
        }

        private (bool isIStem, string stem) IsIStem(NounData input)
        {
            // check if this is an i-stem 

            // split syllables

            var nom = Syllablifier.SyllabifyWord(input.Nominative);
            if (input.GenitiveSingular is null)
                throw new ArgumentException("The word provided must have a known genitive singular form");
            var gen = Syllablifier.SyllabifyWord(input.GenitiveSingular);
            if (nom.Count == gen.Count)
                return (true, StripEnding(input.GenitiveSingular, "is"));
            // nom. form is only 1 syllable long
            if (nom.Count == 1)
                if (input.Nominative.EndsWith("x") || input.Nominative.EndsWith("s"))
                {
                    // check that there are 2 consonants before the ending
                    var stem = StripEnding(input.Nominative, "is");
                    var consonants = "qwrtypsdfghjklzxcvbnm";

                    if (consonants.Contains(stem.Last()) && consonants.Contains(stem[stem.Length - 2].ToString()))
                        return (true, StripEnding(input.GenitiveSingular, "is"));
                }

            // neuter + endings
            if (input.Gender == Gender.Neuter)
                if (input.Nominative.EndsWith("e") || input.Nominative.EndsWith("al") ||
                    input.Nominative.EndsWith("ar"))
                    return (true, StripEnding(input.GenitiveSingular, "is"));

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
            var nom = string.IsNullOrEmpty(nominative) ? stem + endings.Nominative : nominative;
            var acc = endings.Accusative == "-" ? nom : stem + endings.Accusative;
            var gen = endings.Genitive == "-" ? nom : stem + endings.Genitive;
            var dat = endings.Dative == "-" ? nom : stem + endings.Accusative;
            var abl = endings.Ablative == "-" ? nom : stem + endings.Ablative;
            var voc = endings.Vocative == "-" ? nom : stem + endings.Vocative;

            return new Cases(nom, acc, gen, dat, abl, voc);
        }
    }
}