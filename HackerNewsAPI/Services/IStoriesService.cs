using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public interface IStoriesService
    {
        Task<List<Story>> GetFilteredStoriesAsync(SearchRequest searchRequest);
    }
}
