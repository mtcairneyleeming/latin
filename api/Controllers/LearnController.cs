using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using LatinAutoDecline.Database;
using LatinAutoDecline.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/learn")]
    public class LearnController : Controller
    {
        private readonly LatinContext _context;
        private readonly IHelper _helper;

        public LearnController(LatinContext ctx, IHelper helper)
        {
            _context = ctx;
            _helper = helper;
        }

        #region Learning process: methods to get lemmas to learn
        [Authorize]
        [HttpGet("list/{listId}/{learnt}")]
        public Result<IEnumerable<Lemma>> GetLemmasByRevisionStatusInList(int listId, bool learnt)
        {
            var sectionIDs = _context.Sections.Where(s => s.ListId == listId).Select(s => s.ListId);
            // all lemmas in a list
            var lemmaIds = _context.SectionWords.Where(s => sectionIDs.Contains(s.SectionId)).Select(s => s.LemmaId);
            // get learnt lemmas
            var learntIds = _context.UserLearntWords.Where(w => lemmaIds.Contains(w.LemmaId) && w.LearntPercentage > 0).Select(w => w.LemmaId);
            // get not-yet learnt / partially unfinished lemmas in a list
            if (learnt)
            {
                return new Result<IEnumerable<Lemma>>(_helper.LoadLemmasWithData(learntIds));
            }
            var unlearntIds = lemmaIds.Except(learntIds);
            return new Result<IEnumerable<Lemma>>(_helper.LoadLemmasWithData(unlearntIds));
        }

        [Authorize]
        [HttpGet("section/{sectionId}/{learnt}")]
        public Result<IEnumerable<Lemma>> GetLemmasByRevisionStatusInSection(int sectionId, bool learnt)
        {
            // all lemmas in a section
            var lemmaIds = _context.SectionWords.Where(s => s.SectionId == sectionId).Select(s => s.LemmaId);
            // get learnt lemmas
            var learntIds = _context.UserLearntWords.Where(w => lemmaIds.Contains(w.LemmaId)).Select(w => w.LemmaId);
            // get not-yet learnt / partially unfinished lemmas in a list

            return new Result<IEnumerable<Lemma>>(_helper.LoadLemmasWithData(lemmaIds));
        }


        #endregion
        #region Settings lemmas as learnt/not learnt
        [Authorize]
        [HttpDelete("{id}")]
        public EResult DeleteProgress(int id)
        {

            var u = GetCurrentUser();
            var currentLemmaInfo =
                _context.UserLearntWords.FirstOrDefault(w => w.LemmaId == id && w.UserId == u);
            if (currentLemmaInfo is null)
            {
                // Already doesn't exist
                return new EResult(true);
            }
            _context.UserLearntWords.Remove(currentLemmaInfo);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return new EResult("Deleting this lemma failed", currentLemmaInfo, e);
            }
            return new EResult(true);
        }

        // PATCH api/learn/5?add=60
        [Authorize]
        [HttpPatch("{id}")]
        public EResult UpdateProgress(int id, int learntPercentage)
        {
            if (learntPercentage <= 0)
            {
                return new EResult("The extra percentage of this lemma learnt must be above zero", learntPercentage);
            }
            var u = GetCurrentUser();
            var currentLemmaInfo =
                _context.UserLearntWords.FirstOrDefault(w => w.LemmaId == id && w.UserId == u);
            if (currentLemmaInfo is null)
            {
                var userLearntWords = new UserLearntWord
                {
                    LearntPercentage = learntPercentage,
                    LemmaId = id,
                    UserId = u,
                    NextRevision = DateTime.Now.AddDays(7),
                    RevisionStage = 0
                };
                _context.UserLearntWords.Add(userLearntWords);
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    return new EResult("Setting this lemma as learnt for the first time failed.", userLearntWords, e);
                }
                return new EResult(true);
            }
            currentLemmaInfo.LearntPercentage += learntPercentage;
            currentLemmaInfo.NextRevision = DateTime.Now.AddDays(7);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return new EResult("Updating how this lemma was failed", currentLemmaInfo, e);
            }
            return new EResult(true);
        }
        #endregion

        #region Getting lemmas to revise
        #endregion
        #region Setting lemmas as recently revised - 
        [Authorize]
        [HttpPost("{id}/revise")]
        public EResult Post(int id)
        {

            var u = GetCurrentUser();
            Console.WriteLine(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var currentLemmaInfo =
                _context.UserLearntWords.FirstOrDefault(w => w.LemmaId == id && w.UserId == u);
            if (currentLemmaInfo is null)
            {
                return new EResult("This lemma cannot be revised, as it has not been learnt yet", id);
            }
            currentLemmaInfo.NextRevision = DateTime.Now.AddDays(7);
            // TODO implement memrise-like increasing durations between revisions
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return new EResult("Revising this lemma failed", currentLemmaInfo, e);
            }
            return new EResult(true);
        }
        #endregion


        private string GetCurrentUser()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }


    }
}
