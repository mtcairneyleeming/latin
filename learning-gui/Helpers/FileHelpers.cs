using System.Collections.Generic;
using System.Linq;
using database.Database;
using database.Helpers;
using learning_gui.Types;
using Terminal.Gui;

namespace learning_gui.Helpers
{
    public static class FileHelpers
    {
        public static List<Lemma> GenerateData(List<WordList> lists, LatinContext context, bool ignoreUnknown = false)
        {
            var words = new List<Lemma>();


            var allWords = new List<string>();
            allWords.AddRange(lists.SelectMany(l => l.Words));
            var preloadWords = allWords.Where(w => w.Length > 1 && char.IsNumber(w.Last())).ToList();
            var adjustedWords = preloadWords.Select(w => w.Substring(0, w.Length - 1));
            var preloadData = context.Lemmas
                //                .Include(l => l.UserLearntWord)
                //                .Include(l => l.LemmaData).ThenInclude(l => l.PartOfSpeech)
                //                .Include(l => l.LemmaData).ThenInclude(l => l.Gender)
                //                .Include(l => l.LemmaData).ThenInclude(l => l.Category)
                //                .Include(l => l.Definitions)
                .Where(l => adjustedWords.Contains(l.LemmaText))
                .ToList();
            var filteredData = new List<Lemma>();
            foreach (var word in preloadWords)
            {
                var disambiguator = int.Parse(word.Last().ToString()) - 1;
                var options = preloadData.Where(l => l.LemmaText == word.Substring(0, word.Length - 1)).ToList();
                
                if (disambiguator >= options.Count) continue;
                
                var lemma = options[disambiguator];
                if (lemma.UserLearntWord is null) lemma.UserLearntWord = new UserLearntWord {RevisionStage = 0, LemmaId = lemma.LemmaId};
                filteredData.Add(lemma);
            }

            context.SaveChanges();
            words.AddRange(filteredData);


            foreach (var wordList in lists)
            {
                foreach (var t in wordList.Words.ToList())
                {
                    if (preloadWords.Contains(t)) continue;
                    var word = t.Trim();
                    var lemma = GetLemma(word, out var disambiguator, context, ignoreUnknown);
                    if (lemma is null) continue;
                    if (lemma.UserLearntWord is null) lemma.UserLearntWord = new UserLearntWord {RevisionStage = 0, LemmaId = lemma.LemmaId};

                    words.Add(lemma);

                    var newText = lemma.LemmaText + (disambiguator > 0 ? disambiguator.ToString() : "");


                    if (newText == t) continue;

                    wordList.UpdateWord(t, newText);
                }

                context.SaveChanges();
                wordList.Save();
            }

            return words;
        }

        private static Lemma GetLemma(string data, out int disambiguator, LatinContext context, bool ignoreUnknown)
        {
            disambiguator = 1; // default value, which updates the list 
            data = data.Trim();
            if (data.Length < 1) return null;
            Lemma lemma = null;
            var multipleOptions = new List<Lemma>();
            if (char.IsDigit(data.Last()))
            {
                disambiguator = int.Parse(data.Last().ToString());
                var lemmas = context.Lemmas
//                    .Include(l => l.UserLearntWord)
//                    .Include(l => l.LemmaData).ThenInclude(l => l.PartOfSpeech)
//                    .Include(l => l.LemmaData).ThenInclude(l => l.Gender)
//                    .Include(l => l.LemmaData).ThenInclude(l => l.Category)
//                    .Include(l => l.Definitions)
                    .Where(l => l.LemmaText == data.Remove(data.Length - 1))
                    .ToList();
                if (!lemmas.Any()) return null;

                lemma = lemmas[disambiguator - 1];
            }
            else
            {
                switch (context.Lemmas.Count(l => l.LemmaText == data))
                {
                    case 0:
                        // no lemma found, so it needs to be queried in the step below
                        break;
                    case 1:
                        lemma = context.Lemmas
//                            .Include(l => l.UserLearntWord)
//                            .Include(l => l.LemmaData).ThenInclude(l => l.PartOfSpeech)
//                            .Include(l => l.LemmaData).ThenInclude(l => l.Gender)
//                            .Include(l => l.LemmaData).ThenInclude(l => l.Category)
//                            .Include(l => l.Definitions)
                            .FirstOrDefault(l => l.LemmaText == data);
                        break;
                    default:
                        // more than one lemma, so we need to disambiguate

                        multipleOptions.AddRange(
                            context.Lemmas
//                                .Include(l => l.UserLearntWord)
//                                .Include(l => l.LemmaData).ThenInclude(l => l.PartOfSpeech)
//                                .Include(l => l.LemmaData).ThenInclude(l => l.Gender)
//                                .Include(l => l.LemmaData).ThenInclude(l => l.Category)
//                                .Include(l => l.Definitions)
                                .Where(l => l.LemmaText == data)
                                .OrderBy(l => l.LemmaId)
                        );
                        break;
                }
            }

            if (!(lemma is null)) return lemma;

            // ask about multiple options - should only be done on first load of a list, as it will then be saved over.
            if (multipleOptions.Count > 0)
            {
                var buttonTexts = multipleOptions
                    .Select(o => $"{o.LemmaText}:{(o.Definitions.FirstOrDefault()?.Data ?? o.LemmaShortDef).Truncate(18).Trim()}").ToArray();
                var i = MessageBox.Query(100, 6, "Lemma Ambiguity",
                    "Please choose one of the lemmas below to resolve the ambiguity", buttonTexts);
                disambiguator = i + 1;
                return multipleOptions[i];
            }

            // ask about a lemma we can't find, if we have been allowed to.
            if (ignoreUnknown)
                return null;

            var wasCanceled = true;
            var ok = new Button(3, 14, "Ok")
            {
                Clicked = () =>
                {
                    Application.RequestStop();
                    wasCanceled = false;
                }
            };
            var cancel = new Button(10, 14, "Cancel")
            {
                Clicked = Application.RequestStop
            };
            var desc = new Label(
                "A lemma was not found matching the word provided.\n Please fix it below or click Cancel to ignore it")
            {
                Height = 2
            };
            var entry = new TextField(data)
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(),
                Height = 1
            };
            var dialog = new Dialog("Lemma Matching Error", 70, 8, ok, cancel) {desc, entry};
            Application.Run(dialog);

            // it was cancelled, return null
            if (wasCanceled) return null;

            data = entry.Text.ToString();
            return GetLemma(data, out disambiguator, context, false);
        }

        public static void AddDefinitions(string path, bool ignoreUnknown = false)
        {
            var data = WordList.Load(path);
            var skipped = new List<string>();
            using (var context = new LatinContext())
            {
                foreach (var line in data.WordsWithDefinitions)
                {
                    var lemma = GetLemma(line.Key, out _, context, ignoreUnknown);
                    if (lemma is null)
                    {
                        skipped.Add(string.Join(":", line));
                        continue;
                    }

                    if (lemma.Definitions.Count(d => d.Data == line.Value.Trim()) > 0) continue;
                    lemma.Definitions.Add(new Definition
                    {
                        Data = line.Value.Trim(),
                        Level = 1,
                        LemmaId = lemma.LemmaId
                    });
                }

                context.SaveChanges();
            }

            if (skipped.Any())
            {
                MessageBox.ErrorQuery(100, 6 + skipped.Count, "Errors",
                    "The following lemmas were skipped as they could not be found in the database:\n" + string.Join("\n", skipped), "Close");
            }
        }
    }
}