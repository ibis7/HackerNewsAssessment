using HackerNewsAPI.Models;
using HackerNewsAPI.Services.Interfaces;

namespace HackerNewsAPI.Services
{
    public class ThirdPartyService(IHttpClientFactory httpClientFactory, ILogger<ThirdPartyService> logger) : IThirdPartyService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("HackerNews");

        private async Task<T?> ExecuteGetRequestAsync<T>(string endpoint)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<T>(endpoint);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "There was an error trying to communicate with HackerNewsAPI with url {url}", endpoint);
                return default;
            }
        }

        public async Task<Story?> GetStoryDetailsAsync(int storyId)
        {
            return await ExecuteGetRequestAsync<Story>($"item/{storyId}.json");
        }

        public async Task<List<int>> GetNewestStoryIdsAsync()
        {
            return (await ExecuteGetRequestAsync<List<int>>("newstories.json")) ?? [];
        }
    }
}
