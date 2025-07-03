using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public interface ISearchService
    {
        Task<List<Story>> GetFilteredStoriesAsync(SearchRequest searchRequest);
    }
}
