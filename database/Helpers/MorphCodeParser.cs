using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global

namespace database.Helpers
{
    /// <summary>
    ///     Tools to convert DB representations of forms into those used in the decliner and the api
    /// </summary>
    public static class MorphCodeParser
    {
        public static List<string> ParseCode(string morphCode)
        {
            var data = new List<string>();
            for (var i = 0; i < morphCode.Length; i++)
            {
                if (morphCode[i] is '-') continue;
                switch (i)
                {
                    case 0:
                        data.Add(ParsePartOfSpeech(morphCode).ToString());
                        break;
                    case 1:
                        data.Add(ParsePerson(morphCode).ToString());
                        break;
                    case 2:
                        data.Add(ParseNumber(morphCode).ToString());
                        break;
                    case 3:
                        data.Add(ParseTense(morphCode).ToString());
                        break;
                    case 4:
                        data.Add(ParseMood(morphCode).ToString());
                        break;
                    case 5:
                        data.Add(ParseVoice(morphCode).ToString());
                        break;
                    case 6:
                        data.Add(ParseGender(morphCode).ToString());
                        break;
                    case 7:
                        data.Add(ParseCase(morphCode).ToString());
                        break;
                    case 8:
                        data.Add(ParseDegree(morphCode).ToString());
                        break;
                }
            }

            return data;
        }

        public static Part? ParsePartOfSpeech(string morphCode)
        {
            var dict = new Dictionary<char, Part>
            {
                {'n', Part.Noun},
                {'v', Part.Verb},
                {'t', Part.Participle},
                {'a', Part.Adjective},
                {'d', Part.Adverb},
                {'c', Part.Conjunction},
                {'r', Part.Preposition},
                {'p', Part.Pronoun},
                {'m', Part.Numeral},
                {'i', Part.Interjection},
                {'e', Part.Exclamation}
            };
            var caseLetter = morphCode[0];
            if (caseLetter == '-') return null;
            return dict[caseLetter];
        }

        public static Person? ParsePerson(string morphCode)
        {
            var caseToProperty = new Dictionary<char, Person>
            {
                {'1', Person.First},
                {'2', Person.Second},
                {'3', Person.Third}
            };
            var caseLetter = morphCode[1];
            if (caseLetter == '-') return null;
            return caseToProperty[caseLetter];
        }

        public static Number? ParseNumber(string morphCode)
        {
            var caseToProperty = new Dictionary<char, Number>
            {
                {'s', Number.Singular},
                {'p', Number.Plural},
                {'d', Number.Plural}
            };
            var caseLetter = morphCode[2];
            if (caseLetter == '-') return null;
            return caseToProperty[caseLetter];
        }

        public static Tense? ParseTense(string morphCode)
        {
            var caseToProperty = new Dictionary<char, Tense>
            {
                {'p', Tense.Present},
                {'i', Tense.Imperfect},
                {'r', Tense.Perfect},
                {'l', Tense.Pluperfect},
                {'t', Tense.FuturePerfect},
                {'f', Tense.Future},
                {'a', Tense.Aorist}
            };
            var caseLetter = morphCode[3];
            if (caseLetter == '-') return null;
            return caseToProperty[caseLetter];
        }

        public static Mood? ParseMood(string morphCode)
        {
            var caseToProperty = new Dictionary<char, Mood>
            {
                {'i', Mood.Indicative},
                {'s', Mood.Subjunctive},
                {'n', Mood.Infinitive},
                {'m', Mood.Imperative},
                {'g', Mood.Gerundive},
                {'u', Mood.Supine},
                {'d', Mood.Gerund},
                {'p', Mood.Participle}
            };
            var caseLetter = morphCode[4];
            if (caseLetter == '-') return null;
            return caseToProperty[caseLetter];
        }

        public static Voice? ParseVoice(string morphCode)
        {
            var caseToProperty = new Dictionary<char, Voice>
            {
                {'a', Voice.Active},
                {'p', Voice.Passive},
                {'d', Voice.Deponent},
                {'e', Voice.MedioPassive}
            };
            var caseLetter = morphCode[5];
            if (caseLetter == '-') return null;
            return caseToProperty[caseLetter];
        }

        public static Gender? ParseGender(string morphCode)
        {
            var caseToProperty = new Dictionary<char, Gender>
            {
                {'m', Gender.Masculine},
                {'f', Gender.Feminine},
                {'n', Gender.Neuter},
                {'-', Gender.Indeterminate}
            };
            var caseLetter = morphCode[6];
            return caseToProperty[caseLetter];
        }

        public static Case? ParseCase(string morphCode)
        {
            // Fixes minor issue where forms are recorded with the wrong morphcode, and don't give the case
            if ((morphCode[7] == '-') & (morphCode == "n-s---m--")) return Case.Nominative;
            var caseToProperty = new Dictionary<char, Case>
            {
                {'n', Case.Nominative},
                {'g', Case.Genitive},
                {'d', Case.Dative},
                {'a', Case.Accusative},
                {'b', Case.Ablative},
                {'v', Case.Vocative},
                {'l', Case.Locative},
                {'i', Case.Instrumental}
            };
            var caseLetter = morphCode[7];
            if (caseLetter == '-') return null;
            return caseToProperty[caseLetter];
        }


        public static Degree? ParseDegree(string morphCode)
        {
            var caseToProperty = new Dictionary<char, Degree>
            {
                {'p', Degree.Positive},
                {'c', Degree.Comparative},
                {'s', Degree.Superlative}
            };
            var caseLetter = morphCode[8];
            if (caseLetter == '-') return null;
            return caseToProperty[caseLetter];
        }
    }
}