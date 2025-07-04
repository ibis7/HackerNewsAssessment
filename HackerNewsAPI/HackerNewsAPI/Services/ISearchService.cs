using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public interface ISearchService
    {
        Task<SearchResponse> GetFilteredStoriesAsync(SearchRequest searchRequest);
    }
}
