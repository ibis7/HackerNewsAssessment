using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public interface IStoriesService
    {
        Task<Story?> GetStoryDetailsAsync(int storyId);
        Task<List<int>> GetNewestStoryIdsAsync();
    }
}
