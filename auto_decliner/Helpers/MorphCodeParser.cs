using System;
using System.Collections.Generic;
using System.Reflection;
using LatinAutoDecline.Database;
using LatinAutoDecline.Nouns;

namespace LatinAutoDecline.Helpers
{
    /// <summary>
    /// Tools to convert DB representations of forms into those used in the decliner and the api
    /// </summary>
    public static class MorphCodeParser
    {
        
        public static NounTable ProcessForms(List<Form> forms)
        {

            var nounTable = new NounTable();
            foreach (var form in forms)
            {
                Type type;
                PropertyInfo prop;
                Case formCase = ParseCase(form.MorphCode);
                if (formCase == Case.Locative || formCase == Case.Instrumental)
                {
                    // Ignore instrumental and locative cases
                    continue;
                }
                Number num = ParseNumber(form.MorphCode);
                switch (num){
                    // Singular
                    case Number.Singular:
                        if (nounTable.SingularCaseTable != null)
                        {
                            type = nounTable.SingularCaseTable.GetType();

                            prop = type.GetProperty(ParseCase(form.MorphCode).ToString());

                            prop.SetValue(nounTable.SingularCaseTable, form.Text, null);
                        }
                        nounTable.UseSingular = true;
                        break;
                    case Number.Plural:
                        if (nounTable.PluralCaseTable != null)
                        {
                            type = nounTable.PluralCaseTable.GetType();

                            prop = type.GetProperty(ParseCase(form.MorphCode).ToString());

                            prop.SetValue(nounTable.PluralCaseTable, form.Text, null);
                        }
                        break;

                }

            }
            return nounTable;
        }


        private static Person ParsePerson(string morphCode)
        {
            Dictionary<char, Person> caseToProperty = new Dictionary<char, Person>
            {
                {'1', Person.First },
                {'2', Person.Second },
                {'3', Person.Third }
                //{'', Gender.Inderterminate },
            };
            char caseLetter = morphCode[1];
            return caseToProperty[caseLetter];
        }
        private static Number ParseNumber(string morphCode)
        {
            Dictionary<char, Number> caseToProperty = new Dictionary<char, Number>
            {
                {'s', Number.Singular },
                {'p', Number.Plural },
                {'d', Number.Plural }
            };
            char caseLetter = morphCode[2];
            return caseToProperty[caseLetter];
        }

        private static Tense ParseTense(string morphCode)
        {
            Dictionary<char, Tense> caseToProperty = new Dictionary<char, Tense>
            {
                {'p', Tense.Present},
                {'i', Tense.Imperfect },
                {'r', Tense.Perfect },
                {'l', Tense.Pluperfect},
                {'t', Tense.FuturePerfect},
                {'f', Tense.Future},
                {'a', Tense.Aorist}
            };
            char caseLetter = morphCode[3];
            return caseToProperty[caseLetter];
        }
        private static Mood ParseMood(string morphCode)
        {
            Dictionary<char, Mood> caseToProperty = new Dictionary<char, Mood>
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
            char caseLetter = morphCode[4];
            return caseToProperty[caseLetter];
        }
        private static Voice ParseVoice(string morphCode)
        {
            Dictionary<char, Voice> caseToProperty = new Dictionary<char, Voice>
            {
                {'a', Voice.Active},
                {'p', Voice.Passive},
                {'d', Voice.Deponent},
                {'e', Voice.MedioPassive}


            };
            char caseLetter = morphCode[5];
            return caseToProperty[caseLetter];
        }
        private static Gender ParseGender(string morphCode)
        {
            Dictionary<char, Gender> caseToProperty = new Dictionary<char, Gender>
            {
                {'m', Gender.Masculine },
                {'f', Gender.Feminine },
                {'n', Gender.Neuter }
            };
            char caseLetter = morphCode[6];
            return caseToProperty[caseLetter];
        }
        private static Case ParseCase(string morphCode)
        {
            Dictionary<char, Case> caseToProperty = new Dictionary<char, Case>
            {
                {'n', Case.Nominative },
                {'g', Case.Genitive },
                {'d', Case.Dative },
                {'a', Case.Accusative },
                {'b', Case.Ablative },
                {'v', Case.Vocative },
                {'l', Case.Locative },
                {'i', Case.Instrumental }
            };
            char caseLetter = morphCode[7];
            return caseToProperty[caseLetter];
        }


        private static Degree ParseDegree(string morphCode)
        {
            Dictionary<char, Degree> caseToProperty = new Dictionary<char, Degree>
            {
                {'p', Degree.Positive},
                {'c', Degree.Comparative},
                {'s', Degree.Superlative}


            };
            char caseLetter = morphCode[8];
            return caseToProperty[caseLetter];
        }

    }
}
