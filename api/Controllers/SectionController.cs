using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using decliner.Database;
using decliner.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("{sectionId}")]
        public Result<Section> GetById(int sectionId)
        {
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            if (section != null)
            {
                if (_context.Lists.Any(l => l.ListId == section.ListId && l.IsPrivate == false)) ;
                {
                    return new Result<Section>(section);
                }
                return new Result<Section>("You must have permission to view that list");
            }
            return new Result<Section>("The section must exist to be accessed");
        }

        [Authorize]
        [HttpPost]
        public Result<Section> Post([FromBody] string name, [FromBody] int listId)
        {
            if (_context.ListUsers.Any(o =>
                o.ListId == listId && o.UserId == GetCurrentUser() && (o.IsContributor || o.IsOwner)))
            {
                var l = new Section {Name = name, ListId = listId};
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
                if (_context.ListUsers.Any(o =>
                    o.ListId == section.ListId && o.UserId == GetCurrentUser() && (o.IsContributor || o.IsOwner)))
                {
                    _context.Sections.Remove(section);
                    _context.SaveChanges();
                    return new EResult();
                }
                return new EResult("You must have permission to modify that list");
            }
            return new EResult("The section must exist to be deleted");
        }

        // Import a text block of lemmas to a list
        [Authorize]
        [HttpPost("{sectionId}/lemmas/import")]
        public Result<LemmaImportResponseModel> ImportLemmas(int sectionId, [FromBody] LemmaImportModel data)
        {
            var lemmasToMatch = data.lemmas;
            if (lemmasToMatch is null || !lemmasToMatch.Any())
                return new Result<LemmaImportResponseModel>("Please provide some data");
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            if (section is null)
                return new Result<LemmaImportResponseModel>(
                    "The section must exist for you to be able to add lemmas to it");
            if (_context.ListUsers.Any(o =>
                o.ListId == section.ListId && o.UserId == GetCurrentUser() && (o.IsContributor || o.IsOwner)))
                return new Result<LemmaImportResponseModel>("You must have permission to modifiy this list");
            var returnData = new LemmaImportResponseModel();
            // basic matching - works for "domus", but not "alii... alii"
            returnData.SuccessfulImports = _context.Lemmas.Where(l => lemmasToMatch.Contains(l.LemmaText)).ToList();
            foreach (var success in returnData.SuccessfulImports)
                lemmasToMatch.Remove(success.LemmaText);
            _context.SaveChanges();
            return new Result<LemmaImportResponseModel>("Not implemented");
        }

        [Authorize]
        [HttpPost("{sectionId}/lemmas")]
        public EResult AddLemmasToSection(int sectionId, [FromBody] LemmaIdsModel data)
        {
            var ids = data.ids;
            if (ids is null || !ids.Any())
            {
                return new EResult("Please provide some data");
            }

            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            if (section is null)
                return new EResult("The section must exist for you to be able to add lemmas to it");

            if (_context.ListUsers.Any(o =>
                o.ListId == section.ListId && o.UserId == GetCurrentUser() && (o.IsContributor || o.IsOwner)))
            {
                var existingLemmaIds =
                    _context.SectionWords.Where(s => s.SectionId == sectionId).Select(s => s.LemmaId);
                var newLemmaIds = ids.Distinct().Where(l => !existingLemmaIds.Contains(l));
                foreach (var id in newLemmaIds)
                    _context.SectionWords.Add(new SectionWord
                    {
                        LemmaId = id,
                        SectionId = sectionId
                    });
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
                return new EResult("Please provide some data");
            }

            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            if (section is null)
                return new EResult("The section must exist for you to be able to modify it");

            if (_context.ListUsers.Any(o =>
                o.ListId == section.ListId && o.UserId == GetCurrentUser() && (o.IsContributor || o.IsOwner)))
            {
                var lemmasToDelete =
                    _context.SectionWords.Where(w => ids.Contains(w.LemmaId) && w.SectionId == sectionId);
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

        public class LemmaImportModel
        {
            public List<string> lemmas { get; set; }
        }

        /// <summary>
        ///     What we return after an import
        /// </summary>
        public class LemmaImportResponseModel
        {
            public int SectionId { get; set; }

            /// <summary>
            ///     the lemmas for which import worked - no extra info provided
            /// </summary>
            public List<Lemma> SuccessfulImports { get; set; }

            /// <summary>
            ///     lemmas with multiple possible DB values - definitions and classifications are provided to allow the user to choose
            ///     the right one
            /// </summary>
            public List<Lemma> AmbiguousLemmas { get; set; }

            /// <summary>
            ///     Words we have no idea about - ask user to select/type in correct word
            /// </summary>
            public List<string> UnknownLemmas { get; set; }
        }

        public class LemmaIdsModel
        {
            public List<int> ids { get; set; }
        }
    }
}