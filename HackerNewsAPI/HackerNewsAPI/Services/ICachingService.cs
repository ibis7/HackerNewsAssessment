using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public interface ICachingService
    {
        Task<List<Story>> GetNewestStoriesAsync();
    }
}
