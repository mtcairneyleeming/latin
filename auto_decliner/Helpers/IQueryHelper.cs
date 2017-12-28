using System.Collections.Generic;
using decliner.Database;

namespace decliner.Helpers
{
    public interface IQueryHelper
    {
        IEnumerable<Lemma> LoadLemmasWithData(IEnumerable<int> ids);
        Lemma LoadLemmaWithData(int id);
        Declension LoadDeclension(LatinContext db, int lemmaId);
        Gender LoadGender(LatinContext db, int lemmaId);
        List<Lemma> GetRandomLemmas(int num);
    }
}