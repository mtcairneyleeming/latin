using System;
using System.Collections.Generic;
using System.Linq;
using database.Database;
using McMaster.Extensions.CommandLineUtils;
using Serilog;

namespace cli
{
    public static class WiktionaryAPILoaders
    {
        public static void BuildLoader(CommandLineApplication app)
        {
            var loadNounDeclensions = BuildComplexWiktionaryLoadCommand(app, "noun", "declension", new Dictionary<int, string>
            {
                {1, "Latin_first_declension_nouns"},
                {2, "Latin_second_declension_nouns"},
                {3, "Latin_third_declension_nouns"},
                {4, "Latin_fourth_declension_nouns"},
                {5, "Latin_fifth_declension_nouns"},
                {0, "Latin_irregular_nouns"}
            }, "D", 1);

            var loadAdjDeclensions = BuildComplexWiktionaryLoadCommand(app, "adjective", "declension", new Dictionary<int, string>
            {
                {6, "Latin_first_and_second_declension_adjectives"},
                {2, "Latin_second_declension_adjectives"},
                {3, "Latin_third_declension_adjectives"}
            }, "D", 3);
            var loadVerbConjugations = BuildComplexWiktionaryLoadCommand(app, "verb", "conjugation", new Dictionary<int, string>
            {
                {1, "Latin_first_conjugation_verbs"},
                {2, "Latin_second_conjugation_verbs"},
                {3, "Latin_third_conjugation_verbs"},
                {4, "Latin_fourth_conjugation_verbs"},
                {0, "Latin_irregular_verbs"}
            }, "V", 2);
            var loadNounGenders = app.Command("noun-genders", LoadNounGenders());
            var loadAdverbs = BuildWiktionaryLoadCommand(app, "adverbs", 8);
            var loadConjunctions = BuildWiktionaryLoadCommand(app, "conjunctions", 4);
            var loadPrepositions = BuildWiktionaryLoadCommand(app, "prepositions", 6);
            var loadPronouns = BuildWiktionaryLoadCommand(app, "pronouns", 7);


            app.Command("all", command =>
            {
                command.Description = "Load all data";
                command.HelpOption("-h|--help");
                var skipOption = command.Option<bool>("--skip", "Skip words with pre-existing data", CommandOptionType.NoValue);
                command.OnExecute(() =>
                {
                    Log.Information("Noun Declensions");
                    loadNounDeclensions.Execute("1", "2", "3", "4", "5", "0", skipOption.Values.Any() ? "--skip" : "");
                    Log.Information("Noun Genders");
                    loadNounGenders.Execute("M", "F", "N", "I", skipOption.Values.Any() ? "--skip" : "");
                    Log.Information("Adj Declensions");
                    loadAdjDeclensions.Execute("6", "3", "2", skipOption.Values.Any() ? "--skip" : "");
                    Log.Information("Adverbs");
                    loadAdverbs.Execute(skipOption.Values.Any() ? "--skip" : "");
                    Log.Information("Verb conjugations");
                    loadVerbConjugations.Execute("1", "2", "3", "4", "0", skipOption.Values.Any() ? "--skip" : "");
                    Log.Information("Conjunctions");
                    loadConjunctions.Execute(skipOption.Values.Any() ? "--skip" : "");
                    Log.Information("Prepositions");
                    loadPrepositions.Execute(skipOption.Values.Any() ? "--skip" : "");
                    Log.Information("Pronouns");
                    loadPronouns.Execute(skipOption.Values.Any() ? "--skip" : "");
                    return 0;
                });
            });
        }

        /// <summary>
        ///     Builds a loader for a part of speech that only has 1 sub-category, e.g. Conjunctions
        /// </summary>
        /// <param name="app">The cli to add this command</param>
        /// <param name="partName">
        ///     The name of the part of speech to add (used as cli subcommand and in the Wiktionary page name
        ///     identifier)
        /// </param>
        /// <param name="dbPartId">The id of the part of speech in the database table learn.part_of_speech</param>
        /// <returns></returns>
        private static CommandLineApplication BuildWiktionaryLoadCommand(CommandLineApplication app, string partName,
            int dbPartId)
        {
            return app.Command(partName, command =>
            {
                command.Description = $"Mark all applicable lemmas as {partName} in the DB";
                command.HelpOption("-h|--help");
                var skipOption = command.Option("-s|--skip", "Skip words with pre-existing data", CommandOptionType.NoValue);

                command.OnExecute(async () =>
                {
                    Log.Information($"Working on {partName}");
                    var data = await WiktionaryTools.GetPageNames("Latin_" + partName);
                    DatabaseUpdater.UpdateCategory(
                        data.Item1,
                        new Dictionary<string, int> {{"PartOfSpeechId", dbPartId}}, skipOption.Values.Any(), data.Item2);
                    return 0;
                });
            });
        }

        private static CommandLineApplication BuildComplexWiktionaryLoadCommand(CommandLineApplication app, string partName, string dataName,
            Dictionary<int, string> categories, string catId, int dbPartId)
        {
            return app.Command($"{partName}-{dataName}s", command =>
            {
                command.Description = $"Load the {dataName} data for all {partName}s";
                command.HelpOption("-h|--help");
                var skipOption = command.Option<bool>("--skip", "Skip words with pre-existing data", CommandOptionType.NoValue);
                var arguments = command.Argument($"[{dataName}s]",
                    $"The {dataName} to load, provided as a list of numbers", true);
                command.OnExecute(async () =>
                {
                    var parsedArgs = arguments.Values.Select(v => Convert.ToInt32(v));
                    var categoriesToUpdate = categories.Where(e => parsedArgs.Contains(e.Key)).Select(e => (e.Key, e.Value)).ToList();
                    var newDataValues = new List<Dictionary<string, int>>();
                    var context = new LatinContext();
                    foreach (var category in categoriesToUpdate)
                    {
                        newDataValues.Add(new Dictionary<string, int>
                        {
                            {
                                "CategoryId", context.Category.First(d =>
                                    d.Number == category.Item1 && d.CategoryIdentifier == catId).CategoryId
                            },
                            {"PartOfSpeechId", dbPartId}
                        });
                    }

                    await DatabaseUpdater.UpdateMultipleCategories(
                        categoriesToUpdate.Select(c => c.Item2).ToList(),
                        WiktionaryTools.GetPageNames,
                        newDataValues, skipOption.Values.Any());
                    return 0;
                });
            });
        }


        private static Action<CommandLineApplication> LoadNounGenders()
        {
            return command =>
            {
                command.Description = "Load the genders for all nouns in each gender";
                command.HelpOption("-h|--help");
                var skipOption = command.Option<bool>("--skip", "Skip words with pre-existing data", CommandOptionType.NoValue);

                var genderArguments = command.Argument("[genders]",
                    "The genders to load, provided as a combination of 'M', 'F', 'N', and 'I'", true);
                command.OnExecute(async () =>
                {
                    var genderCategories = new Dictionary<char, string>
                    {
                        {'F', "Latin_feminine_nouns"},
                        {'M', "Latin_masculine_nouns"},
                        {'N', "Latin_neuter_nouns"},
                        {'I', "Latin_unknown_gender_nouns"}
                    };
                    var arguments = genderArguments.Values.Select(Convert.ToChar);
                    var categoriesToUpdate = genderCategories.Where(e => arguments.Contains(e.Key)).Select(e => (e.Key, e.Value)).ToList();
                    var newDataValues = new List<Dictionary<string, int>>();
                    var context = new LatinContext();
                    foreach (var category in categoriesToUpdate)
                    {
                        newDataValues.Add(new Dictionary<string, int>
                        {
                            {
                                "GenderId", context.Genders.First(d =>
                                    d.GenderCode == category.Item1.ToString()).GenderId
                            },
                            {"PartOfSpeechId", 1} // 1 for a noun
                        });
                    }

                    await DatabaseUpdater.UpdateMultipleCategories(
                        categoriesToUpdate.Select(c => c.Item2).ToList(),
                        WiktionaryTools.GetPageNames,
                        newDataValues, skipOption.Values.Any());

                    return 0;
                });
            };
        }
    }
}