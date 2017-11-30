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
        private static IHelper _helper;
        private static DeclinerTesters _testers;
        private static WiktionaryLoaders _wiktionary;
        private static DefinitionLoaders _definitions;
        
        
        private static Declension LoadCategory(LatinContext db, int lemma)
        {
            // SELECT number FROM link.declensions RIGHT OUTER JOIN learn.nouns ON nouns.declension_id = declensions.declension_id WHERE nouns.lemma_id = 1
            var declension = (Category) (from decl in db.Category
                join noun in db.LemmaData on decl.CategoryId equals noun.CategoryId
                where noun.LemmaId == lemma
                select decl);
            return (Declension) declension.Number;
        }

        private static Gender LoadGender(LatinContext db, int lemma)
        {
            // SELECT number FROM link.genders RIGHT OUTER JOIN learn.nouns ON nouns.declension_id = declensions.declension_id WHERE nouns.lemma_id = 1
            var gender = (Category) (from gend in db.Genders
                join noun in db.LemmaData on gend.GenderId equals noun.GenderId
                where noun.LemmaId == lemma
                select gend);
            return (Gender) gender.Number;
        }

        static void Main(string[] args)
        {
            var _context = new LatinContext();
            _helper = new QueryHelper(_context);
            _testers = new DeclinerTesters(_context);
            _wiktionary = new WiktionaryLoaders();
            _definitions = new DefinitionLoaders();
            var cli = BuildApp();
            cli.Execute(args);
        }

        static CommandLineApplication BuildApp()
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

            var tester = app.Command("test", BuildTester);
            var loader = app.Command("load", BuildLoader);

            return app;
        }

        static void BuildTester(CommandLineApplication command)
        {
            command.Description = "Test the auto decline functions of the LATIN system ";
            command.HelpOption("-h|--help");
            command.ExtendedHelpText =
                "The word type argument should be one of 'noun', [and others to come later] ";

            var wordTypeArg = command.Argument("[wordType]", "Which type of Latin word to decline.");
            var repetitionsOption = command.Option("-r|--repetitions <number>",
                "The number of times to run the test", CommandOptionType.SingleValue);
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
                        _testers.TestNouns(repetitions);
                        break;
                    default:
                        Console.WriteLine(
                            "Unknown word type entered, or implementation has not been completed for that word type yet");
                        return -1;
                }


                return 0;
            });
        }

        static void BuildLoader(CommandLineApplication cli)
        {
            var loadNounDeclensions = cli.Command("noun-declensions", _wiktionary.loadNounDeclensions());
            var loadNounGenders = cli.Command("noun-genders", _wiktionary.loadNounGenders());
            var loadAdjDeclensions = cli.Command("adj-declensions", _wiktionary.loadAdjDeclensions());
            var loadAdverbs = _wiktionary.BuildWiktionaryLoader(cli, "adverbs", "Adverb");
            var loadVerbConjugations = cli.Command("verb-conjugations", _wiktionary.loadVerbConjugations());
            var loadConjunctions = _wiktionary.BuildWiktionaryLoader(cli, "conjunctions", "Conjunction");
            var loadPrepositions = _wiktionary.BuildWiktionaryLoader(cli, "prepositions", "Preposition");

            var loadDefinitions = cli.Command("definitions", _definitions.LoadDefinitions());
            
            var loadAll = cli.Command("all", command =>
            {
                command.Description = "Load all data";
                command.HelpOption("-h|--help");
                command.OnExecute(() =>
                {
                    Console.WriteLine("NounData Declensions:");
                    loadNounDeclensions.Execute("1", "2", "3", "4", "5", "0");
                    Console.WriteLine(("NounData Genders"));
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
            
        }

        

        

       
    }
}