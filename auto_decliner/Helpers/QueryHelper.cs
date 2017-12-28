using System.Collections.Generic;
using System.Data;
using System.Linq;
using decliner.Database;
using Microsoft.EntityFrameworkCore;

namespace decliner.Helpers
{
    public class QueryHelper : IQueryHelper
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
                //.Include(l => l.Definitions)
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
                .Include(l => l.Definitions).FirstOrDefault();
            lemma.Forms = _context.Forms.Where(f => f.LemmaId == lemma.LemmaId).ToList();

            return lemma;
        }

        /// <summary>
        ///     Load the declension of a lemma, in enum form
        /// </summary>
        /// <param name="db"></param>
        /// <param name="lemmaId"></param>
        /// <returns></returns>
        public Declension LoadDeclension(LatinContext db, int lemmaId)
        {
            // SELECT number FROM link.declensions RIGHT OUTER JOIN learn.nouns ON nouns.declension_id = declensions.declension_id WHERE nouns.lemma_id = 1
            var declension = db.Category
                .Join(db.LemmaData, decl => decl.CategoryId, noun => noun.CategoryId, (decl, noun) => new {decl, noun})
                .Where(t => t.noun.LemmaId == lemmaId)
                .Select(t => t.decl).FirstOrDefault();
            if (declension is null) throw new DataException("The noun provided has no declension");
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

        public List<Lemma> GetRandomLemmas(int num)
        {
            return _context.Lemmas.FromSql(@"SELECT TOP {0} * FROM perseus.lemmas ORDER BY NEWID()", num)
                .Include(l => l.Forms).ToList();
        }

        public static List<string> BuildTags(Lemma word)
        {
            var tags = new List<string>
            {
                word.LemmaData.PartOfSpeech.PartName,
                word.LemmaData.Category.Name
            };
            if (word.LemmaData.Gender != null) tags.Append(word.LemmaData.Gender.Name);

            return tags;
        }
    }
}