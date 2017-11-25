using System.Collections.Generic;
using LatinAutoDecline.Database;

namespace LatinAutoDecline.Helpers
{
    public interface IHelper
    {
        IEnumerable<Lemma> LoadLemmasWithData(IEnumerable<int> ids);
        Lemma LoadLemmaWithData(int id);
        Declension LoadCategory(LatinContext db, int lemma);
        Gender LoadGender(LatinContext db, int lemma);
    }
}