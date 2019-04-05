using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using database.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ReSharper disable AccessToDisposedClosure

namespace cli
{
    public static class DatabaseUpdater
    {
        private static readonly ConcurrentDictionary<string, MethodInfo> _getMethodCache = new ConcurrentDictionary<string, MethodInfo>();
        private static readonly ConcurrentDictionary<string, MethodInfo> _setMethodCache = new ConcurrentDictionary<string, MethodInfo>();

        private static readonly List<(int lemmaId, string lemma, string fieldName, int oldVal, int newVal)> _dataConflicts =
            new List<(int lemmaId, string lemma, string fieldName, int oldVal, int newVal)>();

        public static async Task UpdateMultipleCategories(IReadOnlyList<string> labels,
            Func<string, Task<(IEnumerable<string>, int)>> getCategoryMembers,
            IReadOnlyList<Dictionary<string, int>> newData, bool skipPreExisting)
        {
            for (var i = 0; i < labels.Count; i++)
            {
                var categoryName = labels[i];
                Log.Information($"Beginning work on {categoryName}");
                var words = await getCategoryMembers(categoryName);
                UpdateCategory(words.Item1, newData[i], skipPreExisting, words.Item2);
                Log.Information($"Completed {categoryName}");
            }
        }

        public static void UpdateCategory(IEnumerable<string> lemmasToProcess, Dictionary<string, int> newData, bool skipPreExisting, int totalCount)
        {
            foreach (var name in newData.Keys.Distinct())
            {
                _getMethodCache.TryAdd(name, typeof(LemmaData).GetMethod("get_" + name));
                _setMethodCache.TryAdd(name, typeof(LemmaData).GetMethod("set_" + name));
            }

            using (var db = new LatinContext())
            {
                // track lemmas that have already been worked upon by _in this category_ to prevent EF from complaining about duplicate tracking
                var seenLemmas = new List<int>();
                Log.Information($"\tAbout to start on {totalCount} lemmas");
                var current = 0;
                var allPossibleLemmas = db.Lemmas.Include(l => l.LemmaData)
                    .Where(l => lemmasToProcess.Select(r => r.ToLowerInvariant()).Contains(l.LemmaText));
                foreach (var latin in lemmasToProcess)
                {
                    if (current % 100 == 0) Log.Debug($"\t\tWorking on lemma #{current} out of {totalCount}");

                    current++;

                    var possibleLemmas = allPossibleLemmas.Where(l => l.LemmaText == latin.ToLowerInvariant()).ToList();

                    foreach (var lemma in possibleLemmas)
                    {
                        if (lemma is null || seenLemmas.Contains(lemma.LemmaId)) continue;
                        seenLemmas.Add(lemma.LemmaId);

                        var dataInDb = lemma.LemmaData; //db.LemmaData.SingleOrDefault(n => n.LemmaId == lemma.LemmaId);

                        var isNewRecord = false;
                        if (dataInDb == null)
                        {
                            isNewRecord = true;
                            dataInDb = new LemmaData {LemmaId = lemma.LemmaId};
                        }

                        foreach (var (key, value) in newData)
                        {
                            var dbValue = (int?) _getMethodCache[key].Invoke(dataInDb, new object[] { });

                            if (!isNewRecord && !(dbValue is null) && dbValue != value)
                                _dataConflicts.Add((lemma.LemmaId, lemma.LemmaText, key, (int) dbValue, value));

                            _setMethodCache[key].Invoke(dataInDb, new object[] {value});
                        }

                        if (isNewRecord)
                        {
                            db.LemmaData.Add(dataInDb);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.LemmaData.Update(dataInDb);
                            db.SaveChanges();
                        }
                    }
                }

                db.SaveChanges();
                var conflictsByLemma = _dataConflicts.GroupBy(d => d.lemma).Select(g => new
                        {lemma = g.Key, conflicts = g.GroupBy(e => e.lemmaId).Select(f => new {lemma = f.Key, conflicts = f.ToList()}).ToList()})
                    .ToList();
                var skippedCount = 0;
                for (var i = 0; i < conflictsByLemma.Count; i++)
                {
                    var conflicts = conflictsByLemma[i];
                    if (conflicts.conflicts.Count == 1) // if it's a one-off mistake, fix it
                    {
                        foreach (var conflict in conflicts.conflicts)
                        {
                            var lemma = db.Lemmas.Include(l => l.LemmaData).Single(l => l.LemmaId == conflict.lemma);
                            var dataInDb = db.LemmaData.Single(n => n.LemmaId == lemma.LemmaId);
                            foreach (var (_, _, fieldName, _, newVal) in conflict.conflicts)
                                _setMethodCache[fieldName].Invoke(dataInDb, new object[] {newVal});

                            db.LemmaData.Update(dataInDb);
                        }
                    }
                    else if (!skipPreExisting) // if there are multiple options to choose, skip them
                    {
                        Log.Information("\t---------------------------------------\n\tConflict resolution:");
                        foreach (var conflict in conflicts.conflicts)
                        {
                            var lemma = db.Lemmas.Include(l => l.LemmaData).Single(l => l.LemmaId == conflict.lemma);

                            var fieldNames = string.Join(", ", conflict.conflicts.Select(c => c.fieldName));
                            Log.Information(
                                $"\t\tConflict found {i + 1}/{conflictsByLemma.Count}: {lemma.LemmaText} ({lemma.LemmaId}, {lemma.LemmaShortDef}) on its {fieldNames}");

                            var currentValues = string.Join(", ", conflict.conflicts.Select(c =>
                                GetName((int) _getMethodCache[c.fieldName].Invoke(lemma.LemmaData, new object[] { }), c.fieldName, db)));
                            Log.Information($"\t\t\t#1 Current values: {currentValues}");

                            var oldValues = string.Join(", ", conflict.conflicts.Select(c => GetName(c.oldVal, c.fieldName, db)));
                            Log.Information($"\t\t\t#2 Old values:     {oldValues}");

                            var newValues = string.Join(", ", conflict.conflicts.Select(c => GetName(c.newVal, c.fieldName, db)));
                            Log.Information($"\t\t\t#3 New values: {newValues}");

                            var input = Console.ReadLine();
                            if (!int.TryParse(input, out var num)) continue;
                            if (num > 3 || num < 2) continue;

                            var dataInDb = db.LemmaData.Single(n => n.LemmaId == lemma.LemmaId);
                            foreach (var (_, _, fieldName, oldVal, newVal) in conflict.conflicts)
                                _setMethodCache[fieldName].Invoke(dataInDb, num == 2 ? new object[] {oldVal} : new object[] {newVal});

                            db.LemmaData.Update(dataInDb);
                        }
                    }
                    else
                    {
                        skippedCount++;
                    }
                }

                if (skippedCount > 0)
                    Log.Information($"\tSkipped {skippedCount} conflicts");
                db.SaveChanges();
                _dataConflicts.Clear();
                Log.Information($"\tCompleted work on {totalCount} lemmas");
            }
        }

        private static string GetName(int data, string fieldName, LatinContext context)
        {
            switch (fieldName)
            {
                case "GenderId":
                    var gender = context.Genders.FirstOrDefault(g => g.GenderId == data);
                    if (gender is null) return data.ToString();
                    return gender.Name;
                case "CategoryId":
                    var category = context.Category.FirstOrDefault(g => g.CategoryId == data);
                    if (category is null) return data.ToString();
                    return category.Name;
                case "PartOfSpeechId":
                    var part = context.PartOfSpeech.FirstOrDefault(g => g.PartId == data);
                    if (part is null) return data.ToString();
                    return part.PartName;
                default:
                    return data.ToString();
            }
        }
    }
}