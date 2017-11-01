using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LatinAutoDecline;
using LatinAutoDecline.Database;
using LatinAutoDecline.Helpers;
using LatinAutoDecline.Nouns;
using Microsoft.Extensions.CommandLineUtils;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Generators;
using Category = LatinAutoDecline.Database.Category;

namespace LatinAutoDeclineTester
{
    class Program
    {
        /// <summary>
        /// Builds a loader for wiktionary that takes a single category name
        /// </summary>
        /// <param name="app"></param>
        /// <param name="categoryName"></param>
        /// <param name="dbCategoryName"></param>
        /// <returns></returns>
        private static CommandLineApplication BuildWiktionaryLoader(CommandLineApplication app, string categoryName, string dbCategoryName)
        {
            return app.Command("load-" + categoryName, command =>
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

        private static Declension LoadCategory(LatinContext db, int lemma)
        {
            // SELECT number FROM link.declensions RIGHT OUTER JOIN learn.nouns ON nouns.declension_id = declensions.declension_id WHERE nouns.lemma_id = 1
            var declension = (Category)(from decl in db.Category
                                        join noun in db.LemmaData on decl.CategoryId equals noun.CategoryId
                                        where noun.LemmaId == lemma
                                        select decl);
            return (Declension)declension.Number;
        }

        private static Gender LoadGender(LatinContext db, int lemma)
        {
            // SELECT number FROM link.genders RIGHT OUTER JOIN learn.nouns ON nouns.declension_id = declensions.declension_id WHERE nouns.lemma_id = 1
            var gender = (Category)(from gend in db.Genders
                                    join noun in db.LemmaData on gend.GenderId equals noun.GenderId
                                    where noun.LemmaId == lemma
                                    select gend);
            return (Gender)gender.Number;
        }

        static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "latin"
            };
            app.HelpOption("-h|--help");
            app.OnExecute(() =>
            {
                Console.WriteLine("Welcome to the cli for managin the magister latin learning system!");
                Console.WriteLine("To show help, run this command with the flag '-h'");
                return 0;
            });
            var testDecliner = app.Command("test", command =>
            {
                command.Description = "Test the auto decline functions of the LATIN system ";
                command.HelpOption("-h|--help");
                command.ExtendedHelpText =
                    "The word type argument should be one of 'noun', [and others to come later] ";

                var wordTypeArg = command.Argument("[wordType]", "Which type of Latin word to decline.");
                var repetitionsOption = command.Option("-r|--repetitions <number>", "The number of times to run the test", CommandOptionType.SingleValue);
                command.OnExecute(() =>
                {
                    // parse options
                    WordType wordType;
                    Enum.TryParse(wordTypeArg.Value, out wordType);
                    int repetitions = Convert.ToInt32(repetitionsOption.Value());

                    // run
                    switch (wordType)
                    {
                        case WordType.Noun:
                            var decliner = new NounDecliner();
                            decliner.Init();
                            using (var db = new LatinContext())
                            {
                                var lemmas = db.Lemmas.Take(repetitions).ToList();

                                foreach (var lemma in lemmas)
                                {
                                    var forms = db.Forms.Where(f => f.Lemma == lemma).ToList();
                                    var dbTable = MorphCodeParser.ProcessForms(forms);
                                    // load data about noun
                                    Debug.Assert(dbTable.SingularCaseTable != null, "dbTable.SingularCases != null");
                                    var noun = new Noun
                                    {
                                        Nominative = lemma.LemmaText,
                                        GenitiveSingular = dbTable.SingularCaseTable.Value.Genitive,
                                        Declension = LoadCategory(db, lemma.LemmaId),
                                        Gender = LoadGender(db, lemma.LemmaId)
                                    };
                                    var genTable = decliner.Decline(new Noun());
                                }
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown word type entered, or implementation has not been completed for that word type yet");
                            return -1;
                    }


                    return 0;
                });
            });

            // Loading data from wiktionary
            var loadNounDeclensions = app.Command("load-noun-declensions", command =>
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
                        {0, "Latin_irregular_nouns" }
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
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Noun")
                            };
                        },
                        (d, catId, db) =>
                        {
                            d.Category = db.Category.First(c =>
                                c.Number == catId && c.CategoryIdentifier == "D"); // for 'declension'
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Noun");
                        });
                    return 0;
                });
            });
            var loadNounGenders = app.Command("load-noun-genders", command =>
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
                        {'I', "Latin_unknown_gender_nouns" }

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
                                PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Noun")
                            };
                        },
                        (d, catId, db) =>
                        {
                            d.Gender = db.Genders.First(g => g.GenderCode == catId.ToString());
                            d.PartOfSpeech = db.PartOfSpeech.First(p => p.PartName == "Noun");
                        });
                    return 0;

                });
            });
            var loadAdjDeclensions = app.Command("load-adj-declensions", command =>
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
            });
            var loadAdverbs = BuildWiktionaryLoader(app, "adverbs", "Adverb");
            var loadVerbConjugations = app.Command("load-verb-conjugations", command =>
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
            });
            var loadConjunctions = BuildWiktionaryLoader(app, "conjunctions", "Conjunction");
            var loadPrepositions = BuildWiktionaryLoader(app, "prepositions", "Preposition");
            var loadAll = app.Command("load", command =>
            {
                command.Description = "Load all data";
                command.HelpOption("-h|--help");
                command.OnExecute(() =>
                {
                    Console.WriteLine("Noun Declensions:");
                    loadNounDeclensions.Execute("1", "2", "3", "4", "5", "0");
                    Console.WriteLine(("Noun Genders"));
                    loadNounGenders.Execute("M", "F", "N", "I");
                    Console.WriteLine("Adj Declensions");
                    loadAdjDeclensions.Execute("6", "3", "2");
                    Console.WriteLine("Adverbs");
                    loadAdverbs.Execute();
                    Console.WriteLine("Verb conjugations:");
                    loadVerbConjugations.Execute("1", "2", "3", "4", "0");
                    Console.WriteLine("Conjunctions");
                    loadConjunctions.Execute();
                    Console.WriteLine("Prepositions");
                    loadPrepositions.Execute();
                    return 0;
                });
            });

            // loading definitions from gcse/alevel lists
            app.Execute(args);
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
        private static async Task UpdateDb<T>(Dictionary<T, string> categories, List<string> args, Func<Lemma, T, LatinContext, LemmaData> newEntry, Action<LemmaData, T, LatinContext> updateExisting)
        {
            var selectedCategories = args.Select(val => (T)(Convert.ChangeType(val, typeof(T))));
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
        private static void UpdateLemmaData<T>(IEnumerable<Page> pages, T cat, Func<Lemma, T, LatinContext, LemmaData> newEntry, Action<LemmaData, T, LatinContext> updateExisting)
        {
            using (var db = new LatinContext())
            {
                // track lemmas that have already been wokred upon to prevent EF from complaining about duplicate tracking
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
    }
}
