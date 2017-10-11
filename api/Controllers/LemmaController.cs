using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/lemmas")]
    public class LemmaController : Controller
    {
        private readonly latinContext _context;
        public LemmaController(latinContext ctx)
        {
            _context = ctx;

        }
        #region Only lemma(s)
        // GET api/lemmas/5
        [HttpGet("{id}")]
        public Lemmas Get(int id)
        {
            return _context.Lemmas.FirstOrDefault(l => l.LemmaId == id);
        }

        // GET: api/lemmas?ids=41423,41457,42672
        [HttpGet("")]
        public IEnumerable<Lemmas> Get([ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))] IEnumerable<int> values)
        {
            if (values == null)
            {
                return new Lemmas[0];
            }

            var ids = values.ToList();
            return (from l in _context.Lemmas
                    where ids.Contains(l.LemmaId)
                    select l).ToList();
        }
        #endregion
        #region Lemma(s) with optionally category (decl, conj etc) and/or forms
        // GET api/lemmas/extras/5
        [HttpGet("/extras/{id}")]
        public Lemmas GetWithExtra(int id, bool data, bool definition, bool forms)
        {
            var query = _context.Lemmas;
            if (data) query.Include(l => l.LemmaData);
            if (definition) query.Include(l => l.Definition);
            if (forms) query.Include(l => l.Forms);
            return query.FirstOrDefault(l => l.LemmaId == id);
        }

        // GET: api/lemmas/extras?ids=41423,41457,42672
        [HttpGet("/extras")]
        public IEnumerable<Lemmas> GetMultipleWithExtra([ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))] IEnumerable<int> values, bool data, bool definition, bool forms)
        {
            if (values == null)
            {
                return new Lemmas[0];
            }

            var ids = values.ToList();
            var query = (from l in _context.Lemmas
                         where ids.Contains(l.LemmaId)
                         select l);
            if (data) query.Include(l => l.LemmaData).ThenInclude(d => d.Category);
            if (data) query.Include(l => l.LemmaData).ThenInclude(d => d.Gender);
            if (data) query.Include(l => l.LemmaData).ThenInclude(d => d.PartOfSpeech);
            if (definition) query.Include(l => l.Definition);
            if (forms) query.Include(l => l.Forms);

            return query.ToList();
        }
        #endregion




    }
}
