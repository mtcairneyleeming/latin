using System.Collections.Generic;
using System.Linq;
using LatinAutoDecline.Database;
using LatinAutoDecline.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/lemmas")]
    public class LemmaController : Controller
    {
        private readonly LatinContext _context;
        private readonly IHelper _helper;

        public LemmaController(LatinContext ctx, IHelper helper)
        {
            _context = ctx;
            _helper = helper;
        }
        #region Only lemma(s)
        // GET api/lemmas/5
        [HttpGet("{id}")]
        public Lemma Get(int id)
        {
            return _context.Lemmas.FirstOrDefault(l => l.LemmaId == id);
        }

        // GET: api/lemmas?ids=41423,41457,42672
        [HttpGet("")]
        public IEnumerable<Lemma> Get([ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))] IEnumerable<int> values)
        {
            if (values == null)
            {
                return new Lemma[0];
            }

            var ids = values;
            return (from l in _context.Lemmas
                    where ids.Contains(l.LemmaId)
                    select l);
        }
        #endregion
        #region Lemma(s) with optionally category (decl, conj etc) and/or forms
        // GET: api/lemmas/extras?ids=41423,41457,42672
        [HttpGet("extras")]
        public IEnumerable<Lemma> GetMultipleWithExtra([ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))] IEnumerable<int> values)
        {
            if (values == null)
            {
                return new Lemma[0];
            }
            var ids = values.ToList();
            return _helper.LoadLemmasWithData(ids);

        }

        // GET api/lemmas/extras/5
        [HttpGet("extras/{id}")]
        public Lemma GetWithExtra(int id)
        {
            return _helper.LoadLemmaWithData(id);
        }
        #endregion
        [HttpGet("search")]
        public IEnumerable<Lemma> Search(string query)
        {
            return _context.Lemmas.Where(l => EF.Functions.Like(l.LemmaText,  query));
        }
    }
}
