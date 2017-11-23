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
        public Result<Lemma> Get(int id)
        {
            return new Result<Lemma>(_context.Lemmas.FirstOrDefault(l => l.LemmaId == id));
        }

        // GET: api/lemmas?ids=41423,41457,42672
        [HttpGet("")]
        public Result<IEnumerable<Lemma>> Get(
            [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))] IEnumerable<int> values)
        {
            if (values == null)
            {
                return new Result<IEnumerable<Lemma>>(new Lemma[0]);
            }

            var ids = values;
            return new Result<IEnumerable<Lemma>>(
                from l in _context.Lemmas
                where ids.Contains(l.LemmaId)
                select l);
        }

        #endregion

        #region Lemma(s) with optionally category (decl, conj etc) and/or forms

        // GET: api/lemmas/extras?ids=41423,41457,42672
        [HttpGet("extras")]
        public Result<IEnumerable<Lemma>> GetMultipleWithExtra(
            [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))] IEnumerable<int> values)
        {
            if (values == null)
            {
                return new Result<IEnumerable<Lemma>>(new Lemma[0]);
            }
            var ids = values.ToList();
            return new Result<IEnumerable<Lemma>>(_helper.LoadLemmasWithData(ids));
        }

        // GET api/lemmas/extras/5
        [HttpGet("extras/{id}")]
        public Result<Lemma> GetWithExtra(int id)
        {
            return new Result<Lemma>(_helper.LoadLemmaWithData(id));
        }

        #endregion

        [HttpGet("search")]
        public Result<IEnumerable<Lemma>> Search(string query)
        {
            return new Result<IEnumerable<Lemma>>(_context.Lemmas.Where(l => EF.Functions.Like(l.LemmaText, "%" + query + "%")));
        }
    }
}