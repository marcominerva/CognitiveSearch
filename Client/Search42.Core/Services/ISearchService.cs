using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Search42.Core.Models;

namespace Search42.Core.Services
{
    public interface ISearchService
    {
        Task<DocumentSearchResult<CognitiveSearchResult>> SearchAsync(string term, IList<string> facets = null);
    }
}