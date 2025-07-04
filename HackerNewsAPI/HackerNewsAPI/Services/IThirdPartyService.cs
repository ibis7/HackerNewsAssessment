using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public interface IThirdPartyService
    {
        Task<Story?> GetStoryDetailsAsync(int storyId);
        Task<List<int>> GetNewestStoryIdsAsync();
    }
}
