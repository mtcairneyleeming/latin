using System.Collections.Generic;
using System.Linq;
using LatinAutoDecline.Database;
using Microsoft.EntityFrameworkCore;

namespace LatinAutoDecline.Helpers
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
            var lemmas =  _context.Lemmas.Where(l => ids.Contains(l.LemmaId))
                .Include(l => l.LemmaData).ThenInclude(d => d.Category)
                .Include(l => l.LemmaData).ThenInclude(d => d.Gender)
                .Include(l => l.LemmaData).ThenInclude(d => d.PartOfSpeech)
                //.Include(l => l.Definition)
                .ToList();
            for (int i = 0; i < lemmas.Count; i++)
            {
                lemmas[i].Forms = _context.Forms.Where(f => f.LemmaId == lemmas[i].LemmaId).ToList();
            }
            return lemmas;
        }

        public Lemma LoadLemmaWithData(int id)
        {
            Lemma lemma = _context.Lemmas.Where(l => l.LemmaId == id)
                .Include(l => l.LemmaData).ThenInclude(d => d.Category)
                .Include(l => l.LemmaData).ThenInclude(d => d.Gender)
                .Include(l => l.LemmaData).ThenInclude(d => d.PartOfSpeech)
                .Include(l => l.Definition).FirstOrDefault();
            lemma.Forms = _context.Forms.Where(f => f.LemmaId == lemma.LemmaId).ToList();
            
            return lemma;
        }
    }
}
