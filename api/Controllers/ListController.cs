using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using LatinAutoDecline.Database;
using LatinAutoDecline.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/lists")]
    public class ListController : Controller
    {
        private readonly LatinContext _context;
        private readonly IHelper _helper;

        public ListController(LatinContext ctx, IHelper helper)
        {
            _context = ctx;
            _helper = helper;
        }
        // DELETE api/values/5
        [Authorize]
        [HttpDelete("{id}")]
        public EResult Delete(int id)
        {
            var list = _context.Lists.FirstOrDefault(l => l.ListId == id);
            if (list is null)
            {
                return new EResult("Cannot delete a list that does not exist");
            }
            if (list.Users.Any(o => o.UserId == GetCurrentUser() && o.IsOwner))
            {
                _context.Lists.Remove(list);
                return new EResult(true);
            }
            return new EResult("You cannot delete this list as you do not own it");
        }

        // GET: api/lists?search=alevel%20latin%20ocr (optional search)
        [Authorize]
        [HttpGet]
        public IEnumerable<List> Get(string search)
        {
            // TODO: implement w/ auth
            return new List<List>();
        }

        // GET api/lists/5 - direct request - does not require searching permission
        [Authorize]
        [HttpGet("{id}")]
        public Result<List> Get(int id)
        {
            var u = GetCurrentUser();
            var list = _context.Lists.FirstOrDefault(l => l.ListId == id && 
                                                          ((l.IsPrivate && l.Users.Any(o=> o.UserId == u))
                                                          || l.IsPrivate == false));
            if (list is null)
            {
                return new Result<List>("No list could be found with this id. This may be due to a lack of permission to access the lsit with this id.");
            }
            return new Result<List>(list);
        }
        // create new list, but only with the attributes of the list itself
        // POST api/lists
        [Authorize]
        [HttpPost]
        public EResult Post([FromBody]string name, [FromBody] string description)
        {
            var l = new List { Description = description, Name = name, Users = new List<ListUser> { new ListUser { UserId = GetCurrentUser(), IsOwner = true } } };
            _context.Lists.Add(l);
            _context.SaveChanges();
            return new EResult(true);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public EResult Put(int id, [FromBody]string name, [FromBody] string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new EResult("Please provide a name for this list");
            }

            var list = _context.Lists.FirstOrDefault(l =>
                l.ListId == id && l.Users.Any(o => o.UserId == GetCurrentUser() && (o.IsOwner || o.IsContributor)));
            if (list is null)
            {
                return new EResult("The list could not be modified. This may be because it does not exist, or you do not have permission to modify it.");

            }
            list.Name = name;
            list.Description = description;
            _context.SaveChanges();
            return new EResult(true);
        }

        [Authorize]
        [HttpGet("{listId}/lemmas")]
        public Result<IEnumerable<Lemma>> GetLemmasInList(int listId)
        {
            var sectionIDs = _context.Sections.Where(s => s.ListId == listId).Select(s => s.ListId).ToList();
            var lemmaIds = _context.SectionWords.Where(s => sectionIDs.Contains(s.SectionId)).Select(s => s.LemmaId);
            return new Result<IEnumerable<Lemma>>(_helper.LoadLemmasWithData(lemmaIds));
        }
        [Authorize]
        [HttpGet("{listId}/sections")]
        public Result<IEnumerable<Section>> GetSectionsInList(int listId)
        {
            return new Result<IEnumerable<Section>>(_context.Sections.Where(s => s.ListId == listId));
        }



        private string GetCurrentUser()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
