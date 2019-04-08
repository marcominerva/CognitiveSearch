using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Search42.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search42.Core.Services
{
    public class SearchService : ISearchService
    {
        private readonly ISearchIndexClient index;

        public SearchService(string serviceName, string indexName, string apiKey)
        {
            index = new SearchIndexClient(serviceName, indexName, new SearchCredentials(apiKey));
        }

        public async Task<DocumentSearchResult<CognitiveSearchResult>> SearchAsync(string term, IList<string> facets = null)
        {
            var searchParameters = new SearchParameters()
            {
                IncludeTotalResultCount = true,
                Facets = facets
            };

            var results = await index.Documents.SearchAsync<CognitiveSearchResult>(term, searchParameters);
            return results;
        }
    }
}
