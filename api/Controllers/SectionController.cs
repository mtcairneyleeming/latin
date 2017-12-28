using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using decliner.Database;
using decliner.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global 
// used to hide errors on DTOs

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/sections")]
    public class SectionController : Controller
    {
        private readonly LatinContext _context;

        private readonly IQueryHelper
            _helper;

        public SectionController(LatinContext ctx, IQueryHelper helper)
        {
            _context = ctx;
            _helper = helper;
        }

        [Authorize]
        [HttpGet("{sectionId}")]
        public Result<Section> GetById(int sectionId)
        {
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            if (section != null)
            {
                if (_context.Lists.Any(l => l.ListId == section.ListId && l.IsPrivate == false))
                    return new Result<Section>(section);

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

            return new Result<Section>("You must have permission to modify that list in order to add a section to it");
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
            var returnData = new LemmaImportResponseModel
            {
                // basic matching - works for "domus", but not "alii... alii"
                SuccessfulImports = _context.Lemmas.Where(l => lemmasToMatch.Contains(l.LemmaText)).ToList()
            };
            foreach (var success in returnData.SuccessfulImports)
                lemmasToMatch.Remove(success.LemmaText);
            _context.SaveChanges();
            return new Result<LemmaImportResponseModel>("Not fully implemented");
        }

        [Authorize]
        [HttpPost("{sectionId}/lemmas")]
        public EResult AddLemmasToSection(int sectionId, [FromBody] LemmaIdsModel data)
        {
            var ids = data.ids;
            if (ids is null || !ids.Any()) return new EResult("Please provide some data");

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
            if (ids is null || !ids.Any()) return new EResult("Please provide some data");

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

        // TODO: needs security
        [Authorize]
        [HttpGet("{sectionId}/lemmas")]
        public Result<IEnumerable<Lemma>> GetLemmasInSection(int sectionId)
        {
            var lemmaIds = _context.SectionWords.Where(s => s.SectionId == sectionId).Select(s => s.LemmaId).ToList();
            var lemmas = _context.Lemmas.Where(l => lemmaIds.Contains(l.LemmaId));
            return new Result<IEnumerable<Lemma>>(lemmas);
        }

        [Authorize]
        [HttpGet("{sectionId}/learn")]
        public Result<LemmaLearnModel> Learn(int sectionId)
        {
            var ret = new LemmaLearnModel();

            var section = _context.Sections.Where(s => s.SectionId == sectionId).Include(s => s.List).FirstOrDefault();
            if (section is null) return new Result<LemmaLearnModel>("The section must exist for you to learn it");

            var list = section.List;
            if (list.IsPrivate)
                if (!_context.ListUsers.Any(u => u.UserId == GetCurrentUser()))
                    return new Result<LemmaLearnModel>("You must be authorised to learn this section");

            var lemmaIds = _context.SectionWords.Where(w => w.SectionId == sectionId).Select(w => w.LemmaId).ToList();
            var words = _helper.LoadLemmasWithData(lemmaIds).ToList();
            // Filter out other definitions
            words.ForEach(
                w => { w.Definitions = w.Definitions.Where(d => d.Level == list.DefinitionLevel).ToList(); });
            var rand = new Random();
            foreach (var word in words)
            {
                var n = rand.Next(0, 200);
                // TODO: check/tinker with proportional chance
                if (n < 25) // latin --> english translation
                {
                    var answers = new List<string>();
                    word.Definitions.Select(d => d.Data).ToList().ForEach(d => answers.AddRange(d.Split(";")));
                    // Translate latin form --> english
                    ret.TranslationTests.Add(new TranslationTest
                    {
                        Prompt = word.Forms.Select(f => f.Text).OrderBy(f => rand.Next()).FirstOrDefault(),
                        Answers = answers,
                        ExtraShownInfo = "", // TODO: for now ? maybe other stuff
                        Tags = QueryHelper.BuildTags(word)
                    });
                }
                else if (n < 50) // english --> latin translation (ans. with any form of lemma)
                {
                    ret.TranslationTests.Add(new TranslationTest
                    {
                        Answers = word.Forms.Select(f => f.Text).ToList(),
                        Prompt = word.Definitions.FirstOrDefault()?.Data,
                        ExtraShownInfo = "", // TODO: for now ? maybe other stuff
                        Tags = QueryHelper.BuildTags(word)
                    });
                }
                else if (n < 100) // provide correct form of a lemma given the word, definition and required properties
                {
                    var form = word.Forms.OrderBy(f => rand.Next()).FirstOrDefault();
                    if (form is null) continue; // can't test without any forms to test on

                    ret.FormTests.Add(new FormTest
                    {
                        OriginalWord = word.LemmaText,
                        Answer = form.Text,
                        Properties = MorphCodeParser.ParseCode(form.MorphCode),
                        Tags = QueryHelper.BuildTags(word),
                        ExtraShownInfo = word.Definitions.Select(d => d.Data).FirstOrDefault()
                    });
                }
                else if (n < 150
                ) // tap correct form(s) - occaisonally chance may mean that several forms of the same lemma are present
                {
                    var possAns = new List<string>();
                    possAns.Add(word.Forms.OrderBy(f => rand.Next()).Select(f => f.Text).First());
                    var lemmas = _helper.GetRandomLemmas(10);
                    foreach (var lemma in lemmas)
                        possAns.Add(lemma.Forms.OrderBy(f => rand.Next()).Select(f => f.Text).First());

                    ret.TapTests.Add(new TapTest
                    {
                        Prompt = word.Definitions.Select(d => d.Data).FirstOrDefault(),
                        Answers = word.Forms.Select(f => f.Text).ToList(),
                        PossibleAnswers = possAns,
                        Tags = QueryHelper.BuildTags(word)
                    });
                }
                else
                {
                    var pairs = new List<(string question, string ans)>
                    {
                        (word.LemmaText, word.Definitions.FirstOrDefault()?.Data)
                    };
                    var randoms = _helper.GetRandomLemmas(4);
                    foreach (var lemma in randoms)
                        pairs.Add((lemma.LemmaText, word.Definitions.FirstOrDefault()?.Data));

                    ret.MatchTests.Add(new MatchTest
                    {
                        Pairs = pairs,
                        Tags = new List<string>()
                    });
                }
            }

            return new Result<LemmaLearnModel>(ret);
        }


        private string GetCurrentUser()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public class LemmaLearnModel
        {
            /// <summary>
            ///     Takes all forms/individual definitions as answers, can be in either direction
            /// </summary>
            public List<TranslationTest> TranslationTests { get; set; }

            /// <summary>
            ///     Asks for a particular form of a word, only from the latin, e.g. Nom Acc Sing of domus (domum),
            ///     or Indic. Act. Pres. 3rd. Sing of amo (amant)
            /// </summary>
            public List<FormTest> FormTests { get; set; }

            /// <summary>
            ///     Tests where several answers are shown, and the correct one(s) must be clicked:
            ///     - when the answers are in Latin, the possible answers will be drawn from the forms DB,
            ///     so the correct answer may not be that given in the lemma data
            ///     - when the answers are in English, the possible answers will be individual definitions of words in
            ///     the list
            ///     N.B. there is provision for multiple correct answers: e.g. 2 forms, but this will likely not be used currently
            /// </summary>
            public List<TapTest> TapTests { get; set; }

            /// <summary>
            ///     Several pairs of lemmas (1st sing nom/other base form) and definitions are provided, scrambled, and the user needs
            ///     to match them up appropriately
            /// </summary>
            public List<MatchTest> MatchTests { get; set; }
        }

        public class TestBase
        {
            public List<string> Tags { get; set; }
            public string ExtraShownInfo { get; set; }
        }

        public class TranslationTest : TestBase
        {
            public string Prompt { get; set; }
            public List<string> Answers { get; set; }
        }

        public class FormTest : TestBase
        {
            public List<string> Properties { get; set; }
            public string OriginalWord { get; set; }
            public string Answer { get; set; }
        }

        public class TapTest : TranslationTest
        {
            public List<string> PossibleAnswers { get; set; }
        }

        public class MatchTest : TestBase
        {
            /// <summary>
            ///     N.B. correctly sorted, thus the client must rearrange the answers to provide the actual test
            /// </summary>
            public List<(string question, string answer)> Pairs { get; set; }
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