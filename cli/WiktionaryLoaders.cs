using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LatinAutoDecline.Database;
using Microsoft.Extensions.CommandLineUtils;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Generators;

namespace LatinAutoDeclineTester
{
    public class WiktionaryLoaders
    {
        /// <summary>
        /// Builds a loader for wiktionary that takes a single category name
        /// </summary>
        /// <param name="app"></param>
        /// <param name="categoryName"></param>
        /// <param name="dbCategoryName"></param>
        /// <returns></returns>
        public CommandLineApplication BuildWiktionaryLoader(CommandLineApplication app, string categoryName,
            string dbCategoryName)
        {
            return app.Command(categoryName, command =>
            {
                command.Description = $"Mark all applicable lemmas as {categoryName} in the DB";
                command.HelpOption("-h|--help");

                command.OnExecute(async () =>
                {
                    UpdateLemmaData(
                        await GetPages("Latin_" + categoryName),
                        0,
                        (lemma, catId, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = null,
                                Gender = null,
                                // assumption: TODO
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == dbCategoryName)
                            };
                        },
                        (d, catId, db) =>
                        {
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == dbCategoryName);
                        });
                    return 0;
                });
            });
        }

        private static async Task<IEnumerable<Page>> GetPages(string categoryName)
        {
            var wikiClient = new WikiClient();
            var site = await Site.CreateAsync(wikiClient, "https://en.wiktionary.org/w/api.php");
            var gendMembers = new CategoryMembersGenerator(site, "Category:" + categoryName)
            {
                MemberTypes = CategoryMemberTypes.Page
            };
            var pages = gendMembers.EnumPagesAsync().ToEnumerable();
            return pages;
        }

        /// <summary>
        /// Loop through a list of categories and run <see cref="UpdateLemmaData{T}"/> on it
        /// </summary>
        /// <typeparam name="T">The type of category identifer - numbers for declensions, chars for gender</typeparam>
        /// <param name="categories">All possible categories that might be input</param>
        /// <param name="args">user-provided arguments</param>
        /// <param name="newEntry">function to run when the entry is new</param>
        /// <param name="updateExisting">action for updating an existing <see cref="LemmaData"/> entry</param>
        /// <returns></returns>
        private static async Task UpdateDb<T>(Dictionary<T, string> categories, List<string> args,
            Func<Lemma, T, LatinContext, LemmaData> newEntry, Action<LemmaData, T, LatinContext> updateExisting)
        {
            var selectedCategories = args.Select(val => (T) (Convert.ChangeType(val, typeof(T))));
            // load data for each category
            foreach (var cat in selectedCategories)
            {
                Console.WriteLine(categories[cat]);
                var pages = await GetPages(categories[cat]);
                UpdateLemmaData(pages, cat, newEntry, updateExisting);
            }
        }

        /// <summary>
        ///  Update the data for a particular category and run the callbacks for each page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pages"></param>
        /// <param name="cat"></param>
        /// <param name="newEntry"></param>
        /// <param name="updateExisting"></param>
        private static void UpdateLemmaData<T>(IEnumerable<Page> pages, T cat,
            Func<Lemma, T, LatinContext, LemmaData> newEntry, Action<LemmaData, T, LatinContext> updateExisting)
        {
            using (var db = new LatinContext())
            {
                // track lemmas that have already been worked upon to prevent EF from complaining about duplicate tracking
                var seenLemmas = new List<int>();
                foreach (var p in pages)
                {
                    var lemma = db.Lemmas.FirstOrDefault(l => l.LemmaText == p.Title.ToLowerInvariant());
                    if (!(lemma is null) && !(seenLemmas.Contains(lemma.LemmaId)))
                    {
                        seenLemmas.Add(lemma.LemmaId);
                        //Debug.WriteLine($"LemmaID: #{lemma.LemmaId}");

                        LemmaData dataInDb = db.LemmaData.SingleOrDefault(n => n.LemmaId == lemma.LemmaId);
                        if (dataInDb == null)
                        {
                            db.LemmaData.Add(newEntry(lemma, cat, db));
                        }
                        else
                        {
                            updateExisting(dataInDb, cat, db);
                        }
                    }
                }
                db.SaveChanges();
            }
        }
        
        public Action<CommandLineApplication> loadNounDeclensions()
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
                    await UpdateDb(
                        declCategories,
                        declensionArguments.Values,
                        (lemma, catId, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = db.Category.First(d =>
                                    d.Number == catId && d.CategoryIdentifier == "D"),
                                Gender = null,
                                // assumpltion: TODO
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "NounData")
                            };
                        },
                        (d, catId, db) =>
                        {
                            d.Category = db.Category.First(c =>
                                c.Number == catId && c.CategoryIdentifier == "D"); // for 'declension'
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "NounData");
                        });
                    return 0;
                });
            };
        }
        public Action<CommandLineApplication> loadVerbConjugations()
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
                    await UpdateDb(
                        conjCategories,
                        conjugationArgs.Values,
                        (lemma, catId, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = db.Category.First(d =>
                                    d.Number == catId && d.CategoryIdentifier == "V"),
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
        public Action<CommandLineApplication> loadAdjDeclensions()
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
                    await UpdateDb(
                        declCategories,
                        declensionArguments.Values,
                        (lemma, catId, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = db.Category.First(d =>
                                    d.Number == catId && d.CategoryIdentifier == "D"),
                                Gender = null,
                                // assumption: TODO
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Adjective")
                            };
                        },
                        (d, catId, db) =>
                        {
                            d.Category = db.Category.First(c =>
                                c.Number == catId && c.CategoryIdentifier == "D"); // for 'declension'
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
                var declensionArguments = command.Argument("[genders]",
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
                    await UpdateDb(
                        genderCategories,
                        declensionArguments.Values,
                        (lemma, catId, db) =>
                        {
                            return new LemmaData
                            {
                                Lemma = lemma,
                                Category = null,
                                Gender = db.Genders.First(g => g.GenderCode == catId.ToString()),
                                // assumption: TODO
                                UseSingular = true,
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "NounData")
                            };
                        },
                        (d, catId, db) =>
                        {
                            d.Gender = db.Genders.First(g => g.GenderCode == catId.ToString());
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "NounData");
                        });
                    return 0;
                });
            };
        }
    }
}