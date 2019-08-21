using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using database;
using database.Database;
using database.Helpers;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Terminal.Gui;

namespace learning_gui.Helpers
{
    public static class LearningHelpers
    {
        [NotNull] private static readonly Random Rnd = new Random();

        public static List<string> PrincipalParts(Lemma lemma)
        {
            var parts = new List<string>() {lemma.LemmaText};

            if (lemma.LemmaData.PartOfSpeechId == null) return parts;

            switch ((Part) lemma.LemmaData.PartOfSpeechId)
            {
                case Part.Noun:
                    break; // so as not to spoil the declension question
                case Part.Verb:
                    var morphCodes = new List<string>() {"v--pna---", "v1sria---", "n-s-u-nn-"};
                    var extras = morphCodes.Select(m =>
                        lemma.Forms.Where(f => f.MorphCode == m).OrderBy(f => f.Text.Length).FirstOrDefault()
                            ?.Text);
                    parts.AddRange(extras);
                    break;
                case Part.Adjective:
                    break; // so as not to spoil the decl. question
                case Part.Conjunction:
                    break; // no others
                case Part.Numeral:
                    break; // no others
                case Part.Preposition:
                    break; // no others
                case Part.Pronoun:
                    break; // no others/spoilers
                case Part.Adverb:
                    break; // no others/spoilers
                case Part.Interrogative:
                    break; // no other
                case Part.Interjection:
                    break; // no other
            }

            return parts;
        }

        public static Lemma SelectWord([NotNull] IEnumerable<Lemma> words)
        {
            Lemma currentWord;
            words = words.ToList();
            if (!words.Any()) return null;
            while (true)
            {
                var max = words.Max(w => w.UserLearntWord.RevisionStage);
                currentWord = words.OrderBy(w => Rnd.Next(w.UserLearntWord.RevisionStage, max + 1)).First();
                if (currentWord.LemmaData is null)
                {
                    var ok = new Button(3, 14, "Ok")
                    {
                        Clicked = Application.RequestStop
                    };
                    var dialog = new Dialog($"No data found on lemma {currentWord.LemmaText} ({currentWord.LemmaId})",
                        70, 7, ok);
                    Application.Run(dialog);
                    continue;
                }

                break;
            }

            return currentWord;
        }

        public static (Form form, bool success, bool treatedAdjAsParticiple) SelectForm(Lemma l, LatinContext context,
            IEnumerable<string> formsToBlacklist)
        {
            // select forms that aren't blacklisted (any already used) and any indeclinable forms, like adverbs based off a noun
            var forms = context.Forms.Where(f =>
                    f.LemmaId == l.LemmaId && f.Text != l.LemmaText && !formsToBlacklist.Contains(f.Text) &&
                    f.MiscFeatures != "indeclform")
                .ToList();

            var treatedAdjAsParticiple = false;

            if (forms.Count == 0)
            {
                // one cause leading to this is that we have selected a ppp like attonitus that has no forms of its own, but attonito does, just its past participle forms.
                var lemma = context.Lemmas.Single(newL => newL.LemmaId == l.LemmaId);
                var newId = context.Forms.FirstOrDefault(f => f.Text == lemma.LemmaText && f.MorphCode.StartsWith("t"))
                    ?.LemmaId;
                if (newId is null) return (null, false, false);

                forms = context.Forms.Where(f =>
                    f.LemmaId == newId && f.Text != l.LemmaText && EF.Functions.Like(f.MorphCode, "t-_r-____") &&
                    f.MiscFeatures != "indeclform").ToList();
                treatedAdjAsParticiple = true;
                // if still none, return nothing
                if (forms.Count == 0) return (null, false, true);
            }
            else
            {
                // remove any that end in "ve" or "que" by grouping by morphcode and choosing the shortest (i.e. the one without anything stuck on the end)
                var formGroups = forms.GroupBy(f => f.MorphCode).Select(g => g.OrderBy(f => f.Text.Length)).ToList();
                forms = formGroups.Select(g => g.OrderBy(f => f.Text.Length).First()).ToList();
            }


            // should never actually get here, as if there are no forms we have already returned unsuccessfully
            return ChooseFormByWeight(forms, treatedAdjAsParticiple);
            //return (weightedForms.Select(f => new {f.f, rnd = _rnd.Next() * 1/f.weight}).OrderBy(o => o.rnd).First().f, true, treatedAdjAsParticiple);
        }

        private static (Form form, bool success, bool treatedAdjAsParticiple) ChooseFormByWeight(List<Form> forms,
            bool treatedAdjAsParticiple)
        {
            forms = forms.GroupBy(f => f.Text).Select(group => group.First()).ToList();
            var weightedForms = forms.Select(f => (weight: GetWeight(f.MorphCode), f)).ToList();
            var totalWeight = weightedForms.Sum(tuple => tuple.weight);
            var random = Rnd.NextDouble() * totalWeight;
            foreach (var (weight, f) in weightedForms)
            {
                if (random < weight)
                {
                    return (f, true, treatedAdjAsParticiple);
                }

                random -= weight;
            }

            return (null, false, false);
        }

        private static double GetWeight(string morphCode)
        {
            var weight = 1.0;
            if (MorphCodeParser.ParseDegree(morphCode) == Degree.Comparative) weight = 0.5;
            if (MorphCodeParser.ParseDegree(morphCode) == Degree.Superlative) weight = 0.5;


            if (MorphCodeParser.ParsePartOfSpeech(morphCode) == Part.Participle)
            {
                if (MorphCodeParser.ParseTense(morphCode) == Tense.Present) weight = 0.7;
                if (MorphCodeParser.ParseTense(morphCode) == Tense.Perfect) weight = 0.7;
                if (MorphCodeParser.ParseTense(morphCode) == Tense.Future) weight = 0.7;
            }

            if (MorphCodeParser.ParsePartOfSpeech(morphCode) == Part.Verb)
            {
                if (MorphCodeParser.ParseMood(morphCode) == Mood.Gerundive) weight = 0.6;
                if (MorphCodeParser.ParseMood(morphCode) == Mood.Subjunctive) weight *= 1.7;
                if (MorphCodeParser.ParseMood(morphCode) == Mood.Infinitive &&
                    MorphCodeParser.ParseTense(morphCode) != Tense.Present) weight = 1.8;
                if (MorphCodeParser.ParseMood(morphCode) == Mood.Imperative) weight = 1.0;
            }

            return weight;
        }

        public static string SplitTextIntoLines(string text, int width)
        {
            return string.Join("\n", Regex.Matches(text, $".{{1,{width}}}").Select(m => m.Value));
        }
    }
}