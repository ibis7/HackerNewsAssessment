using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public interface ISearchService
    {
        Task<SearchResponse> GetFilteredNewestStoriesAsync(SearchRequest searchRequest);
    }
}
