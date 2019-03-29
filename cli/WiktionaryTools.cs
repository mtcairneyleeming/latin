using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WikiClientLibrary.Client;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Pages.Queries;
using WikiClientLibrary.Pages.Queries.Properties;
using WikiClientLibrary.Sites;

namespace cli
{
    public static class WiktionaryTools
    {
        public static async Task<(IEnumerable<string>, int)> GetPageNames(string categoryName)
        {
            var wikiClient = new WikiClient();
            var site = new WikiSite(wikiClient, "https://en.wiktionary.org/w/api.php");
            await site.Initialization;
            var pageGenerator = new CategoryMembersGenerator(site, "Category: " + categoryName)
            {
                MemberTypes = CategoryMemberTypes.Page
            };
            var cat = new WikiPage(site, "Category:" + categoryName);
            var provider = WikiPageQueryProvider.FromOptions(PageQueryOptions.None);
            provider.Properties.Add(new CategoryInfoPropertyProvider());
            await cat.RefreshAsync(provider);
            //Log.Debug($"Category Category:{categoryName} has {cat.GetPropertyGroup<CategoryInfoPropertyGroup>().PagesCount} pages within it");
            Log.Debug("\tLoading pages under category " + categoryName);
            var pages = pageGenerator.EnumItemsAsync().ToEnumerable().Select(p => p.Title).ToList();
            Log.Debug("\tFinished loading pages");
            return (pages, cat.GetPropertyGroup<CategoryInfoPropertyGroup>().PagesCount);
        }
    }
}