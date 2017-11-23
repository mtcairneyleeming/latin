using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using LatinAutoDecline.Database;
using LatinAutoDecline.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/lists?search=alevel%20latin%20ocr
        [Authorize]
        [HttpGet("search")]
        public Result<IEnumerable<List>> Search(string query)
        {
            if (query == null)
            {
                return new Result<IEnumerable<List>>("Please provide a search query");
            }
            ;
            var data = _context.Lists.Where(l =>
                ((l.IsPrivate && l.Users.Any(o => o.UserId == GetCurrentUser()))
                 || l.IsPrivate == false)
                &&
                (EF.Functions.Like(l.Name, "%" + query + "%") || EF.Functions.Like(l.Description, "%" + query + "%")));
            return new Result<IEnumerable<List>>(data);
        }

        // GET api/lists/5 - direct request - does not require searching permission
        [Authorize]
        [HttpGet("{id}")]
        public Result<List> Get(int id)
        {
            var u = GetCurrentUser();
            var list = _context.Lists.FirstOrDefault(l => l.ListId == id &&
                                                          ((l.IsPrivate && l.Users.Any(o => o.UserId == u))
                                                           || l.IsPrivate == false));
            if (list is null)
            {
                return new Result<List>(
                    "No list could be found with this id. This may be due to a lack of permission to access the list with this id.");
            }
            return new Result<List>(list);
        }

        // GET: api/lists/mine
        [Authorize]
        [HttpGet("mine")]
        public Result<IEnumerable<List>> GetMyLists(string query)
        {
            IEnumerable<List> data;
            if (query == null)
            {
                data = _context.Lists.Where(l =>
                    l.IsPrivate && l.Users.Any(o => o.UserId == GetCurrentUser())
                    || l.IsPrivate == false);
            }
            else
            {
                data = _context.Lists.Where(l =>
                    ((l.IsPrivate && l.Users.Any(o => o.UserId == GetCurrentUser()))
                     || l.IsPrivate == false)
                    &&
                    (EF.Functions.Like(l.Name, "%" + query + "%") ||
                     EF.Functions.Like(l.Description, "%" + query + "%")));
            }
            return new Result<IEnumerable<List>>(data);
        }

        public class ListCreationData
        {
            // ReSharper disable InconsistentNaming
            // ReSharper disable UnassignedField.Global
            public string name;

            public string description;
            // ReSharper restore UnassignedField.Global
            // ReSharper restore InconsistentNaming
        }

        // create new list, but only with the attributes of the list itself
        // POST api/lists
        [Authorize]
        [HttpPost]
        public Result<List> Post([FromBody] ListCreationData data)
        {
            var l = new List
            {
                Description = data.description,
                Name = data.name,
                Users = new List<ListUser> {new ListUser {UserId = GetCurrentUser(), IsOwner = true}}
            };
            _context.Lists.Add(l);
            _context.SaveChanges();
            return new Result<List>(l);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public Result<List> Put(int id, [FromBody] string name, [FromBody] string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new Result<List>("Please provide a name for this list");
            }

            var list = _context.Lists.FirstOrDefault(l =>
                l.ListId == id && l.Users.Any(o => o.UserId == GetCurrentUser() && (o.IsOwner || o.IsContributor)));
            if (list is null)
            {
                return new Result<List>(
                    "The list could not be modified. This may be because it does not exist, or you do not have permission to modify it.");
            }
            list.Name = name;
            list.Description = description;
            _context.SaveChanges();
            return new Result<List>(list);
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