using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LatinAutoDecline.Database;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LatinAutoDecline.Helpers;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/sections")]
    public class SectionController : Controller
    {
        private readonly LatinContext _context;
        private readonly IHelper 
            _helper;

        public SectionController(LatinContext ctx, IHelper helper)
        {
            _context = ctx;
            _helper = helper;
        }
        // create new section, but only with the attributes of the section itself

        [Authorize]
        [HttpPost]
        public Result<Section> Post([FromBody]string name, [FromBody] int listId)
        {
            if (_context.ListUsers.Any(o => o.ListId == listId && o.UserId == GetCurrentUser() && (o.IsContributor || o.IsOwner)))
            {
                var l = new Section { Name = name, ListId = listId };
                _context.Sections.Add(l);
                _context.SaveChanges();
                return new Result<Section>(l);
            }
            return new Result<Section>("You must have permission to modify that list");
            
        }
        [Authorize]
        [HttpDelete("{sectionId}")]
        public EResult Delete(int sectionId)
        {
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            if (section != null)
            {
                if (_context.ListUsers.Any(o => o.ListId == section.ListId && o.UserId == GetCurrentUser() && (o.IsContributor || o.IsOwner)))
                {

                    _context.Sections.Remove(section);
                    _context.SaveChanges();
                    return new EResult();
                }
                return new EResult("You must have permission to modify that list");
            }
            return new EResult("The section must exist to be deleted");

            

        }

        public class LemmaIdsModel
        {
            public List<int> ids { get; set; }
        }
        
        [Authorize]
        [HttpPost("{sectionId}/lemmas")]
        public EResult AddLemmasToSection(int sectionId, [FromBody] LemmaIdsModel data)
        {
            var ids = data.ids;
            if (ids is null || !ids.Any())
            {
                Console.WriteLine(ids);
                return new EResult("Please provide some data");

            }
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            if (section is null)
            {
                return new EResult("The section must exist for you to be able to add lemmas to it");
            }
            if (_context.ListUsers.Any(o => o.ListId == section.ListId && o.UserId == GetCurrentUser()&& (o.IsContributor || o.IsOwner)))
            {
                var existingLemmaIds = _context.SectionWords.Where(s => s.SectionId == sectionId).Select(s => s.LemmaId);
                var newLemmaIds = ids.Distinct().Where(l => !existingLemmaIds.Contains(l));
                foreach (var id in newLemmaIds)
                {
                    _context.SectionWords.Add(new SectionWord
                    {
                        LemmaId = id,
                        SectionId = sectionId
                    });
                }
                _context.SaveChanges();
                return new EResult(true);
            }
            return new EResult("You must have permission to modifiy this list");
        }
        [Authorize]
        [HttpPatch("{sectionId}/lemmas")]
        public EResult RemoveLemmasFromSection(int sectionId, [FromBody] LemmaIdsModel data)
        {
            var ids = data.ids;
            if (ids is null || !ids.Any())
            {
                Console.WriteLine(ids);
                return new EResult("Please provide some data");

            }
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            if (section is null)
            {
                return new EResult("The section must exist for you to be able to modify it");
            }
            if (_context.ListUsers.Any(o => o.ListId == section.ListId && o.UserId == GetCurrentUser()&& (o.IsContributor || o.IsOwner)))
            {
                var lemmasToDelete = _context.SectionWords.Where(w => ids.Contains(w.LemmaId) && w.SectionId == sectionId);
                _context.SectionWords.RemoveRange(lemmasToDelete);
                _context.SaveChanges();
                return new EResult(true);
            }
            return new EResult("You must have permission to modifiy this list");
        }

        [Authorize]
        [HttpGet("{sectionId}/lemmas")]
        public Result<IEnumerable<Lemma>> GetLemmasInSection(int sectionId)
        {
            var lemmaIds = _context.SectionWords.Where(s => s.SectionId == sectionId).Select(s => s.LemmaId).ToList();
            var lemmas = _context.Lemmas.Where(l => lemmaIds.Contains(l.LemmaId));
            return new Result<IEnumerable<Lemma>>(lemmas);
        }


        private string GetCurrentUser()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
