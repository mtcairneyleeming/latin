using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using database.Database;

namespace learning_gui.Helpers
{
    public static class AnswerHelpers
    {
        public static bool CheckEnglishAnswer(string userAns, List<string> answers)
        {
            return answers.Contains(Regex.Replace(userAns.Trim(), @"\s", " "));
        }

        public static List<string> GenerateAnswers(string lemmaShortDef, IEnumerable<Definition> lemmaDefinitions)
        {
            var possAns = new List<string>();
            possAns.AddRange(SplitAnswers(lemmaShortDef));
            possAns.AddRange(lemmaDefinitions.SelectMany(ld => SplitAnswers(ld.Data)));
            possAns = possAns.Select(pa => Regex.Replace(pa, @"\s+", " ")).ToList();
            return possAns.Distinct().ToList();
        }

        private static IEnumerable<string> SplitAnswers(string answerString)
        {
            return OptionalPronoun(OptionalPunctuation(OptionalBrackets(answerString.Split(",").SelectMany(s => s.Split(";")))));
        }

        private static IEnumerable<string> OptionalBrackets(IEnumerable<string> input)
        {
            return input.SelectMany(i => new[]
            {
                i.Trim(),
                Regex.Replace(i, @"(\s*\[\s*)|(\s*\]\s*)|(\s*""\s*)|(\s*'\s*)|(\s*\(\s*)|(\s*\)\s*)", " ").Trim(),
                Regex.Replace(i, @"(\[.*\])|("".*"")|('.*')|(\(.*\))", "").Trim()
            }).ToList();
        }


        private static IEnumerable<string> OptionalPronoun(IEnumerable<string> input)
        {
            return input.SelectMany(i => new[]
                {i.Trim(), Regex.Replace(i, @"(\bI\b)|(\bi\b)|(\bA\b)|(\ba\b)|(\ban\b)|(\ban\b)|(\bto\b)|(\bTo\b)", "").Trim()}).ToList();
        }

        private static IEnumerable<string> OptionalPunctuation(IEnumerable<string> input)
        {
            return input.SelectMany(i => new[] {i.Trim(), Regex.Replace(i, @"(\s*\?\s*)|(\s*:\s*)|(\s*@\s*)|(\s*!\s*)", "").Trim()}).ToList();
        }
    }
}