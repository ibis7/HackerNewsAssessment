using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAPI.Services
{
    public class StoriesService(IMemoryCache cache, IHttpClientFactory httpClientFactory, ILogger<StoriesService> logger) : IStoriesService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("HackerNews");

        public async Task<List<Story>> GetFilteredStoriesAsync(SearchRequest searchRequest)
        {
            //Implement search
            //Implement sorting
            //Implement pagination
            //Implement caching

            var stories = new List<Story>();
            var newestStoriesIds = await GetNewestStoryIdsAsync();

            var filteredStories = newestStoriesIds
                .Skip(searchRequest.PageSize * searchRequest.PageNumber)
                .Take(searchRequest.PageSize)
                .ToList();

            foreach (var storyId in filteredStories)
            {
                var story = await GetStoryDetailsAsync(storyId);
                if (story != null)
                {
                    stories.Add(story);
                }
            }
            return stories;
        }

        private async Task<Story?> GetStoryDetailsAsync(int storyId)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<Story>($"item/{storyId}.json");
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "There was an error trying to communicate with HackerNewsAPI");
                return null;
            }
        }

        private async Task<List<int>> GetNewestStoryIdsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<int>>("newstories.json");
                if (result == null)
                {
                    return [];
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "There was an error trying to communicate with HackerNewsAPI");
                return [];
            }
        }
    }
}
