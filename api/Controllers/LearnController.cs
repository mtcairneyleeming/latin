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
    [Route("api/learn")]
    public class LearnController : Controller
    {
        private readonly LatinContext _context;

        private readonly IQueryHelper _helper;

        // Map stages to revision time: as users progress, they need to see words less often
        private readonly Dictionary<int, int> StageToRevisionTimeSpan = new Dictionary<int, int>
        {
            {0, 1},
            {1, 4},
            {2, 7},
            {3, 14},
            {4, 21},
            {5, 28},
            {6, 35}
        };

        public LearnController(LatinContext ctx, IQueryHelper helper)
        {
            _context = ctx;
            _helper = helper;
        }


        private string GetCurrentUser()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
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
            var learntIds = _context.UserLearntWords.Where(w => lemmaIds.Contains(w.LemmaId))
                .Select(w => w.LemmaId);
            // get not-yet learnt / partially unfinished lemmas in a list
            if (learnt)
                return new Result<IEnumerable<Lemma>>(_helper.LoadLemmasWithData(learntIds));
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
                return new EResult(true);
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

        // sets a lemma as learnt/revised: e.g. first learn/regular revision: if they get it right, it goes up a level, 
        // the time period between revisions becomes longer, up to a maximum
        [Authorize]
        [HttpPatch("{id}")]
        public EResult Learn(int id, bool upLevel)
        {
            var u = GetCurrentUser();
            var currentLemmaInfo =
                _context.UserLearntWords.FirstOrDefault(w => w.LemmaId == id && w.UserId == u);
            if (currentLemmaInfo is null)
            {
                var userLearntWords = new UserLearntWord
                {
                    LemmaId = id,
                    UserId = u,
                    NextRevision = DateTime.Now.AddDays(StageToRevisionTimeSpan[0]),
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

            currentLemmaInfo.RevisionStage += upLevel ? 1 : 0;
            if (currentLemmaInfo.RevisionStage > 6) currentLemmaInfo.RevisionStage = 6;
            currentLemmaInfo.NextRevision =
                DateTime.Now.AddDays(StageToRevisionTimeSpan[currentLemmaInfo.RevisionStage]);
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
    }
}