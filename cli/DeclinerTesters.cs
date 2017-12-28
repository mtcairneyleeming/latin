using System;
using System.Diagnostics;
using System.Linq;
using decliner;
using decliner.Database;
using decliner.Helpers;
using decliner.Nouns;
using decliner.Tables;

namespace cli
{
    public class DeclinerTesters
    {
        private readonly IQueryHelper _helper;

        public DeclinerTesters(LatinContext _context)
        {
            _helper = new QueryHelper(_context);
        }

        public void TestNouns(int repetitions)
        {
            var decliner = new NounDecliner();
            decliner.Init();
            using (var db = new LatinContext())
            {
                var rnd = new Random();
                var lemmas = db.Lemmas.Where(l => l.LemmaData.PartOfSpeech.PartId == (int) Part.Noun).Take(repetitions);


                foreach (var lemma in lemmas)
                {
                    var forms = db.Forms.Where(f => f.Lemma == lemma).ToList();
                    Console.WriteLine(lemma.LemmaText);
                    var dbTable = FormsParser.ProcessNoun(forms);
                    // load data about noun
                    Debug.Assert(dbTable.SingularCases != null, "dbTable.Singular != null");


                    if (dbTable.SingularCases != null)
                    {
                        var noun = new NounData
                        {
                            Nominative = lemma.LemmaText,
                            GenitiveSingular = dbTable.SingularCases.Genitive,
                            Declension = _helper.LoadDeclension(db, lemma.LemmaId),
                            Gender = _helper.LoadGender(db, lemma.LemmaId)
                        };
                        Noun genTable;
                        try
                        {
                            genTable = decliner.Decline(noun);
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine("Malformed input, ignoring");
                            continue;
                        }

                        // TODO: finish tester
                        var r = Comparators.Compare(dbTable, genTable);
                        Console.WriteLine($"{r.Count} differences:");
                        foreach (var diff in r)
                            Console.WriteLine($"{diff.Property}: {diff.FirstVal} vs {diff.SecondVal}");
                    }
                }
            }
        }
    }
}