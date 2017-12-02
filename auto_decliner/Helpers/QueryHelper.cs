using System.Collections.Generic;
using System.Linq;
using decliner.Database;
using Microsoft.EntityFrameworkCore;

namespace decliner.Helpers
{
    public class QueryHelper : IHelper
    {
        private readonly LatinContext _context;

        public QueryHelper(LatinContext ctx)
        {
            _context = ctx;
        }

        public IEnumerable<Lemma> LoadLemmasWithData(IEnumerable<int> ids)
        {
            var lemmas = _context.Lemmas.Where(l => ids.Contains(l.LemmaId))
                .Include(l => l.LemmaData).ThenInclude(d => d.Category)
                .Include(l => l.LemmaData).ThenInclude(d => d.Gender)
                .Include(l => l.LemmaData).ThenInclude(d => d.PartOfSpeech)
                //.Include(l => l.Definition)
                .ToList();
            for (var i = 0; i < lemmas.Count; i++)
                lemmas[i].Forms = _context.Forms.Where(f => f.LemmaId == lemmas[i].LemmaId).ToList();
            return lemmas;
        }

        public Lemma LoadLemmaWithData(int id)
        {
            var lemma = _context.Lemmas.Where(l => l.LemmaId == id)
                .Include(l => l.LemmaData).ThenInclude(d => d.Category)
                .Include(l => l.LemmaData).ThenInclude(d => d.Gender)
                .Include(l => l.LemmaData).ThenInclude(d => d.PartOfSpeech)
                .Include(l => l.Definition).FirstOrDefault();
            lemma.Forms = _context.Forms.Where(f => f.LemmaId == lemma.LemmaId).ToList();

            return lemma;
        }

        /// <summary>
        /// Load the declension of a lemma, in enum form
        /// </summary>
        /// <param name="db"></param>
        /// <param name="lemmaId"></param>
        /// <returns></returns>
        public Declension LoadDeclension(LatinContext db, int lemmaId)
        {
            // SELECT number FROM link.declensions RIGHT OUTER JOIN learn.nouns ON nouns.declension_id = declensions.declension_id WHERE nouns.lemma_id = 1
            var declension = (from decl in db.Category
                join noun in db.LemmaData on decl.CategoryId equals noun.CategoryId
                where noun.LemmaId == lemmaId
                select decl).FirstOrDefault();
//            if (declension is null)
//            {
//                return null;
//            }
            return (Declension) declension.Number;
        }


        public Gender LoadGender(LatinContext db, int lemmaId)
        {
            // SELECT number FROM link.genders RIGHT OUTER JOIN learn.nouns ON nouns.declension_id = declensions.declension_id WHERE nouns.lemma_id = 1
            var gender = (from gend in db.Genders
                join noun in db.LemmaData on gend.GenderId equals noun.GenderId
                where noun.LemmaId == lemmaId
                select gend).FirstOrDefault();
            switch (gender.GenderCode)
            {
                    case "M":
                        return Gender.Masculine;
                    case "F":
                        return Gender.Feminine;
                    case "N":
                        return Gender.Neuter;
                    case "I":
                        return Gender.Indeterminate;
                    default:
                        return Gender.Indeterminate;
            }
        }
    }
}