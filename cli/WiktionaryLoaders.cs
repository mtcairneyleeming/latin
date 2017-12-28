using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using decliner.Database;
using Microsoft.Extensions.CommandLineUtils;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Generators;

// ReSharper disable ArgumentsStyleAnonymousFunction
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleOther


/*TERMINOLOGY:
     attributes of the LemmaData type are referred to as attributes
     the values to give to these attributes are labels e.g. Feminine, Noun, or First Declension
     categories are the categories on Wiktionary for groups of words, e.g. 2nd/3rd Declension adjectives
*/


namespace cli
{
    public class WiktionaryLoaders
    {
        /// <summary>
        ///     Builds a loader for a part of speechthat only has 1 sub-category, e.g. Conjunctions, but not Verbs (first/second/.../irregular)
        /// </summary>
        /// <param name="app">The cli to add this command</param>
        /// <param name="partName">The name of the part of speech to add (used as cli subcommand and in the Wiktionary page name identifier)</param>
        /// <param name="dbPartName">The name of the part of speech in the database table learn.part_of_speech</param>
        /// <returns></returns>
        public CommandLineApplication BuildWiktionaryLoadCommand(CommandLineApplication app, string partName,
            string dbPartName)
        {
            return app.Command(partName, command =>
            {
                command.Description = $"Mark all applicable lemmas as {partName} in the DB";
                command.HelpOption("-h|--help");

                command.OnExecute(async () =>
                {
                    UpdateCategory(
                        lemmas: await GetPageNames("Latin_" + partName),
                        label: 0,
                        onNewEntry: (lemma, catId, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = null,
                                Gender = null,
                                // assumption: TODO either work this out, or just remove this attribute
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == dbPartName)
                            };
                        },
                        updateExisting: (d, catId, db) =>
                        {
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == dbPartName);
                        });
                    return 0;
                });
            });
        }

        /// <summary>
        /// Return the names of pages within a Wiktionary category
        /// </summary>
        /// <param name="categoryName">The name of the Wiktionary category to enumerate</param>
        /// <returns></returns>
        private static async Task<IEnumerable<string>> GetPageNames(string categoryName)
        {
            var wikiClient = new WikiClient();
            var site = await Site.CreateAsync(wikiClient, "https://en.wiktionary.org/w/api.php");
            var gendMembers = new CategoryMembersGenerator(site, "Category: " + categoryName)
            {
                MemberTypes = CategoryMemberTypes.Page
            };
            var pages = gendMembers.EnumPagesAsync().ToEnumerable().Select(p => p.Title);
            return pages;
        }

        /// <summary>
        ///     Loop through a list of categories and run <see cref="UpdateCategory{T}" /> on it
        /// </summary>
        /// <typeparam name="T">Allows for different types, an int or char to be used to identify a label in the database</typeparam>
        /// <param name="labels">The labels to update, e.g. 1st, 2nd and 3rd Decl, w/ their category names</param>
        /// <param name="newEntry">function to run when the entry is new</param>
        /// <param name="updateExisting">action for updating an existing <see cref="LemmaData" /> entry</param>
        /// <returns></returns>
        private static async Task UpdateMultipleCategories<T>(IEnumerable<(T label, string categoryName)> labels,
            Func<Lemma, T, LatinContext, LemmaData> newEntry, Action<LemmaData, T, LatinContext> updateExisting)
        {
            foreach (var label in labels)
            {
                var pages = await GetPageNames(label.categoryName);
                UpdateCategory(pages, label.label, newEntry, updateExisting);
            }
        }

        /// <summary>
        ///     Update the data for a particular category and run the callbacks for each lemma
        /// </summary>
        /// <typeparam name="T">Allows for different types, an int or char to be used to identify a label in the database</typeparam>
        /// <param name="lemmas"></param>
        /// <param name="label">the value to update the attribute being set to: e.g. Masculine for the Gender attribute</param>
        /// <param name="onNewEntry">Function to run wihen the data for the lemma does not yet exist</param>
        /// <param name="updateExisting">Function to run to update pre-existing data</param>
        private static void UpdateCategory<T>(IEnumerable<string> lemmas, T label,
            Func<Lemma, T, LatinContext, LemmaData> onNewEntry, Action<LemmaData, T, LatinContext> updateExisting)
        {
            using (var db = new LatinContext())
            {
                // track lemmas that have already been worked upon to prevent EF from complaining about duplicate tracking
                var seenLemmas = new List<int>();
                foreach (var latin in lemmas)
                {
                    var lemma = db.Lemmas.FirstOrDefault(l => l.LemmaText == latin.ToLowerInvariant());
                    if (lemma is null || seenLemmas.Contains(lemma.LemmaId)) continue;
                    seenLemmas.Add(lemma.LemmaId);

                    var dataInDb = db.LemmaData.SingleOrDefault(n => n.LemmaId == lemma.LemmaId);
                    if (dataInDb == null)
                        db.LemmaData.Add(onNewEntry(lemma, label, db));
                    else
                        updateExisting(dataInDb, label, db);
                }

                db.SaveChanges();
            }
        }

        public Action<CommandLineApplication> LoadNounDeclensions()
        {
            return command =>
            {
                command.Description = "Load the declension data for all nouns in a declension";
                command.HelpOption("-h|--help");
                var declensionArguments = command.Argument("[declensions]",
                    "The declensions to load, provided as a list of numbers", true);
                command.OnExecute(async () =>
                {
                    // parse urls
                    var declCategories = new Dictionary<int, string>
                    {
                        {1, "Latin_first_declension_nouns"},
                        {2, "Latin_second_declension_nouns"},
                        {3, "Latin_third_declension_nouns"},
                        {4, "Latin_fourth_declension_nouns"},
                        {5, "Latin_fifth_declension_nouns"},
                        {0, "Latin_irregular_nouns"}
                    };
                    var arguments = declensionArguments.Values.Select(v => Convert.ToInt32(v));
                    await UpdateMultipleCategories(
                        declCategories.Where(e => arguments.Contains(e.Key)).Select(e => (e.Key, e.Value)).ToList(),
                        (lemma, catName, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = db.Category.First(d =>
                                    d.Number == catName && d.CategoryIdentifier == "D"),
                                Gender = null,
                                // assumpltion: TODO
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "NounData")
                            };
                        },
                        (d, catName, db) =>
                        {
                            d.Category = db.Category.First(c =>
                                c.Number == catName && c.CategoryIdentifier == "D"); // for 'declension'
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "NounData");
                        });
                    return 0;
                });
            };
        }

        public Action<CommandLineApplication> LoadVerbConjugations()
        {
            return command =>
            {
                command.Description = "Load the conjugation data for all verbs in the provided conjugations";
                command.HelpOption("-h|--help");
                var conjugationArgs = command.Argument("[conjugations]",
                    "The conjugations to load, provided as a list of numbers", true);
                command.OnExecute(async () =>
                {
                    // parse urls
                    var conjCategories = new Dictionary<int, string>
                    {
                        {1, "Latin_first_conjugation_verbs"},
                        {2, "Latin_second_conjugation_verbs"},
                        {3, "Latin_third_conjugation_verbs"},
                        {4, "Latin_fourth_conjugation_verbs"},
                        {0, "Latin_irregular_verbs"}
                    };
                    var arguments = conjugationArgs.Values.Select(v => Convert.ToInt32(v));
                    await UpdateMultipleCategories(
                        conjCategories.Where(e => arguments.Contains(e.Key)).Select(e => (e.Key, e.Value)).ToList(),
                        (lemma, catName, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = db.Category.First(d =>
                                    d.Number == catName && d.CategoryIdentifier == "V"),
                                Gender = null,
                                // assumption: TODO
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Verb")
                            };
                        },
                        (d, catId, db) =>
                        {
                            d.Category = db.Category.First(c =>
                                c.Number == catId && c.CategoryIdentifier == "V"); // for 'declension'
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Verb");
                        });
                    return 0;
                });
            };
        }

        public Action<CommandLineApplication> LoadAdjDeclensions()
        {
            return command =>
            {
                command.Description = "Load the declension data for all adjectives in a declension";
                command.HelpOption("-h|--help");
                var declensionArguments = command.Argument("[declensions]",
                    "The declensions to load, provided as a list of numbers", true);
                command.OnExecute(async () =>
                {
                    // parse urls
                    var declCategories = new Dictionary<int, string>
                    {
                        {6, "Latin_first_and_second_declension_adjectives"},
                        {2, "Latin_second_declension_adjectives"},
                        {3, "Latin_third_declension_adjectives"}
                    };
                    var arguments = declensionArguments.Values.Select(v => Convert.ToInt32(v));
                    await UpdateMultipleCategories(
                        declCategories.Where(e => arguments.Contains(e.Key)).Select(e => (e.Key, e.Value)).ToList(),
                        (lemma, catName, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = db.Category.First(d =>
                                    d.Number == catName && d.CategoryIdentifier == "D"),
                                Gender = null,
                                // assumption: TODO
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Adjective")
                            };
                        },
                        (d, catName, db) =>
                        {
                            d.Category = db.Category.First(c =>
                                c.Number == catName && c.CategoryIdentifier == "D"); // for 'declension'
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Adjective");
                        });
                    return 0;
                });
            };
        }

        public Action<CommandLineApplication> loadNounGenders()
        {
            return command =>
            {
                command.Description = "Load the genders for all nouns in each gender";
                command.HelpOption("-h|--help");
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
                    var arguments = genderArguments.Values.Select(v => Convert.ToInt32(v));
                    await UpdateMultipleCategories(
                        genderCategories.Where(e => arguments.Contains(e.Key)).Select(e => (e.Key, e.Value)).ToList(),
                        (lemma, catName, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = null,
                                Gender = db.Genders.First(g => g.GenderCode == catName.ToString()),
                                // assumption: TODO
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "NounData")
                            };
                        },
                        (d, catName, db) =>
                        {
                            d.Gender = db.Genders.First(g => g.GenderCode == catName.ToString());
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "NounData");
                        });
                    return 0;
                });
            };
        }
    }
}