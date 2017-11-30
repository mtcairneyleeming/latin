using System;
using System.Diagnostics;
using System.Linq;
using LatinAutoDecline;
using LatinAutoDecline.Database;
using LatinAutoDecline.Helpers;
using LatinAutoDecline.Nouns;
using Microsoft.Extensions.CommandLineUtils;

namespace LatinAutoDeclineTester
{
    public class DeclinerTesters
    {
        private IHelper _helper;

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
                var lemmas = db.Lemmas.Take(repetitions).ToList();

                foreach (var lemma in lemmas)
                {
                    var forms = db.Forms.Where(f => f.Lemma == lemma).ToList();
                    var dbTable = MorphCodeParser.ProcessForms(forms);
                    // load data about noun
                    Debug.Assert(dbTable.SingularCases != null, "dbTable.Singular != null");
                    var noun = new NounData
                    {
                        Nominative = lemma.LemmaText,
                        GenitiveSingular = dbTable.SingularCases.Value.Genitive,
                        Declension = _helper.LoadCategory(db, lemma.LemmaId),
                        Gender = _helper.LoadGender(db, lemma.LemmaId)
                    };
                    var genTable = decliner.Decline(noun);
                    // TODO: finish tester
                }
            }
        }
    }
}