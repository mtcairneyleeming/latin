using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class LemmaController : Controller
    {
        private readonly latinContext _context;
        public LemmaController(latinContext ctx)
        {
            _context = ctx;

        }
        // GET: api/lemmas?ids=41423,41457,42672
        [HttpGet]
        public IEnumerable<Lemmas> Get([ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))] IEnumerable<int> values)

        {
            if (values == null)
            {
                Console.WriteLine("Null input");
                return new Lemmas[0];
            }

            var ids = values.ToList();
            Console.WriteLine(ids.Count);
            var query =(from l in _context.Lemmas
                        where ids.Contains(l.LemmaId)
                        select l).ToList();
            Console.WriteLine(query.Count);
            return query;
        }

        // GET api/lemmas/5
        [HttpGet("{id}")]
        public Lemmas Get(int id)
        {
            return _context.Lemmas.FirstOrDefault(l => l.LemmaId == id);
        }

       
    }
}
