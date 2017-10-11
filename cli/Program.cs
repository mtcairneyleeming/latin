using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LatinAutoDecline;
using LatinAutoDecline.Nouns;
using LatinAutoDeclineTester.Models;
using Microsoft.Extensions.CommandLineUtils;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Generators;
using Category = LatinAutoDeclineTester.Models.Category;
using Type = System.Type;

namespace LatinAutoDeclineTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "latin",

            };
            app.HelpOption("-h|--help");
            app.OnExecute(() =>
            {
                Console.WriteLine("Welcome to the LATIN cli!");
                Console.WriteLine("To show help, run this command with the flag '-h'");
                return 0;
            });
            app.Command("test", (command) =>
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
                    LatinAutoDecline.Type wordType;
                    Enum.TryParse(wordTypeArg.Value, out wordType);
                    int repetitions = Convert.ToInt32(repetitionsOption.Value());

                    // run
                    switch (wordType)
                    {
                        case LatinAutoDecline.Type.Noun:
                            var decliner = new NounDecliner();
                            decliner.Init();
                            using (var db = new LatinContext())
                            {
                                var lemmas = db.Lemmas.Take(repetitions).ToList();

                                foreach (var lemma in lemmas)
                                {
                                    var forms = db.Forms.Where(f => f.Lemma == lemma).ToList();
                                    var dbTable = ProcessForms(forms);
                                    // load data about noun
                                    Debug.Assert(dbTable.SingularCaseTable != null, "dbTable.SingularCases != null");
                                    var noun = new Noun()
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
            app.Command("load-declensions", (command) =>
            {
                command.Description = "Load the declension numbers for all lemmas";
                command.HelpOption("-h|--help");
                var declensionArguments = command.Argument("[declensions]",
                    "The declensions to load, provided as a list of numbers", true);
                command.OnExecute(async () =>
                {
                    // parse urls
                    var declCategories = new Dictionary<int, string>()
                    {
                        {1, "Latin_first_declension_nouns"},
                        {2, "Latin_second_declension_nouns"},
                        {3, "Latin_third_declension_nouns"},
                        {4, "Latin_fourth_declension_nouns"},
                        {5, "Latin_fifth_declension_nouns"}
                    };
                    var declensions = declensionArguments.Values.Select(val => Convert.ToInt32(val));
                    using (var db = new LatinContext())
                    {
                        // load data
                        foreach (var decl in declensions)
                        {
                            var wikiClient = new WikiClient();
                            var site = await Site.CreateAsync(wikiClient, "https://en.wiktionary.org/w/api.php");
                            var declMembers = new CategoryMembersGenerator(site, "Category:" + declCategories[decl])
                            {
                                MemberTypes = CategoryMemberTypes.Page
                            };
                            var pages = await declMembers.EnumPagesAsync().Take(10).ToList();
                            foreach (var p in pages)
                            {

                                var lemma = db.Lemmas.FirstOrDefault(l => l.LemmaText == p.Title.ToLowerInvariant());
                                if (lemma is null)
                                {
                                }
                                else
                                {
                                    Console.Write(p.Title.ToLowerInvariant());
                                    Console.WriteLine($", LemmaID: #{lemma.LemmaId}");
                                    LemmaData nounInDB = db.LemmaData.SingleOrDefault(n => n.LemmaId == lemma.LemmaId);
                                    Console.WriteLine(nounInDB is null);
                                    if (nounInDB == null)
                                    {
                                        db.LemmaData.Add(new LemmaData()
                                        {
                                            Lemma = lemma,
                                            Category = db.Category.First(d => d.Number == decl),
                                            Gender = null,
                                            // assunmption: TODO
                                            UseSingular = true
                                        });
                                    }
                                    else
                                    {
                                        nounInDB.Category = db.Category.First(d => d.Number == decl);
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                    return 0;
                });
            });
            app.Execute(args);
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
            // SELECT number FROM link.declensions RIGHT OUTER JOIN learn.nouns ON nouns.declension_id = declensions.declension_id WHERE nouns.lemma_id = 1
            var gender = (Category)(from gend in db.Genders
                join noun in db.LemmaData on gend.GenderId equals noun.GenderId
                where noun.LemmaId == lemma
                select gend);
            return (Gender)gender.Number;
        }

        private static NounTable ProcessForms(List<Forms> forms)
        {

            var nounTable = new NounTable();
            foreach (var form in forms)
            {
                Type type;
                PropertyInfo prop;
                if (form.MorphCode[7] == 'i' || form.MorphCode[7] == 'l')
                {
                    // Ignore instrumental and locative cases
                    continue;
                }
                switch (form.MorphCode[2])
                {
                    // Singular
                    case 's':
                        if (nounTable.SingularCaseTable != null)
                        {
                            type = nounTable.SingularCaseTable.GetType();

                            prop = type.GetProperty(CaseToPropertyName(form.MorphCode));

                            prop.SetValue(nounTable.SingularCaseTable, form.Form, null);
                        }
                        nounTable.UseSingular = true;
                        break;
                    case 'p':
                        if (nounTable.PluralCaseTable != null)
                        {
                            type = nounTable.PluralCaseTable.GetType();

                            prop = type.GetProperty(CaseToPropertyName(form.MorphCode));

                            prop.SetValue(nounTable.PluralCaseTable, form.Form, null);
                        }
                        break;

                }

            }
            return nounTable;
        }

        private static string CaseToPropertyName(string morphCode)
        {
            Dictionary<char, string> caseToProperty = new Dictionary<char, string>()
            {
                {'n', "Nominative" },
                {'g', "Genitive" },
                {'d', "Dative" },
                {'a', "Accusative" },
                {'b', "Ablative" },
                {'v', "Vocative" }
            };
            char caseLetter = morphCode[7];
            return caseToProperty[caseLetter];
        }
    }
}
