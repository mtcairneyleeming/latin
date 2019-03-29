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
        [NotNull] private static readonly Random _rnd = new Random();

        public static Lemma SelectWord([NotNull] IEnumerable<Lemma> words)
        {
            Lemma currentWord;
            words = words.ToList();
            while (true)
            {
                var max = words.Max(w => w.UserLearntWord.RevisionStage);
                currentWord = words.OrderBy(w => _rnd.Next(w.UserLearntWord.RevisionStage, max + 1)).First();
                if (currentWord.LemmaData is null)
                {
                    var ok = new Button(3, 14, "Ok")
                    {
                        Clicked = Application.RequestStop
                    };
                    var dialog = new Dialog($"No data found on lemma {currentWord.LemmaText} ({currentWord.LemmaId})", 70, 7, ok);
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
                    f.LemmaId == l.LemmaId && f.Text != l.LemmaText && !formsToBlacklist.Contains(f.Text) && f.MiscFeatures != "indeclform")
                .ToList();

            var treatedAdjAsParticiple = false;

            if (forms.Count == 0)
            {
                // one cause leading to this is that we have selected a ppp like attonitus that has no forms of its own, but attonito does, just its past participle forms.
                var lemma = context.Lemmas.Single(newL => newL.LemmaId == l.LemmaId);
                var newId = context.Forms.FirstOrDefault(f => f.Text == lemma.LemmaText && f.MorphCode.StartsWith("t"))?.LemmaId;
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

            var weightedForms = forms.Select(f => (weight: GetWeight(f.MorphCode), f)).ToList();
            var totalWeight = weightedForms.Sum(tuple => tuple.weight);
            double random = _rnd.NextDouble() * totalWeight;
            foreach (var (weight, f) in weightedForms)
            {
                if (random < weight)
                {
                    return (f, true, treatedAdjAsParticiple);
                }

                random -= weight;
            }

            // should never actually get here, as if there are no forms we have already returned unsuccessfully
            return (null, false, false);
            //return (weightedForms.Select(f => new {f.f, rnd = _rnd.Next() * 1/f.weight}).OrderBy(o => o.rnd).First().f, true, treatedAdjAsParticiple);
        }

        private static double GetWeight(string morphCode)
        {
            var weight = 1.0;
            if (MorphCodeParser.ParseDegree(morphCode) == Degree.Comparative) weight = 0.5;
            if (MorphCodeParser.ParseDegree(morphCode) == Degree.Superlative) weight = 0.5;


            if (MorphCodeParser.ParsePartOfSpeech(morphCode) == Part.Participle)
            {
                if (MorphCodeParser.ParseTense(morphCode) == Tense.Present) weight = 0.3;
                if (MorphCodeParser.ParseTense(morphCode) == Tense.Perfect) weight = 0.4;
                if (MorphCodeParser.ParseTense(morphCode) == Tense.Future) weight = 0.4;
            }

            if (MorphCodeParser.ParsePartOfSpeech(morphCode) == Part.Verb)
            {
                if (MorphCodeParser.ParseMood(morphCode) == Mood.Gerundive) weight = 1.0;
                if (MorphCodeParser.ParseMood(morphCode) == Mood.Subjunctive) weight *= 1.5;
                if (MorphCodeParser.ParseMood(morphCode) == Mood.Infinitive) weight = 1.0;
                if (MorphCodeParser.ParseMood(morphCode) == Mood.Imperative) weight = 1.0;
            }

            return weight;
        }

        public static string SplitTextIntoLines(string text, int width)
        {
            return String.Join("\n", Regex.Matches(text, $".{{1,{width}}}").Select(m => m.Value));
        }
    }
}