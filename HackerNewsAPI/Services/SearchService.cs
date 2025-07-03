using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAPI.Services
{
    public class SearchService(IMemoryCache cache, IStoriesService storiesService, ILogger<SearchService> logger) : ISearchService
    {
        public async Task<List<Story>> GetFilteredStoriesAsync(SearchRequest searchRequest)
        {
            //Implement search
            //Implement sorting
            //Implement pagination
            //Implement caching

            var stories = new List<Story>();
            var newestStoriesIds = await storiesService.GetNewestStoryIdsAsync();

            var filteredStories = newestStoriesIds
                .Skip(searchRequest.PageSize * searchRequest.PageNumber)
                .Take(searchRequest.PageSize)
                .ToList();

            foreach (var storyId in filteredStories)
            {
                var story = await storiesService.GetStoryDetailsAsync(storyId);
                if (story != null)
                {
                    stories.Add(story);
                }
            }
            return stories;
        }
    }
}
