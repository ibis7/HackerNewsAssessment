using HackerNewsAPI.Models;
using HackerNewsAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAPI.Services
{
    public class CachingService(IMemoryCache cache, IThirdPartyService thirdPartyService, ILogger<CachingService> logger) : ICachingService
    {
        private const string CachedStories = "NewestStories";
        private const int MaxCachedStoriesCount = 500;
        private static readonly TimeSpan BaseCacheDuration = TimeSpan.FromMinutes(20);

        public async Task<List<Story>> GetNewestStoriesAsync()
        {
            if (cache.TryGetValue(CachedStories, out List<Story>? cachedStories) && cachedStories != null)
            {
                logger.LogInformation("CACHING: Requested from cached stories.");
                return await GetCachedStoriesAsync(cachedStories);
            }
            else
            {
                logger.LogInformation("CACHING: Cached stories are unavailable/expired.");
                return await GetNewStoriesAsync();
            }
        }

        private async Task<List<Story>> GetCachedStoriesAsync(List<Story> currentCachedStories)
        {
            var newestStoryIds = await thirdPartyService.GetNewestStoryIdsAsync();

            var newUncachedStoryIds = newestStoryIds.Except(currentCachedStories.Select(x => x.Id)).ToList();

            if (newUncachedStoryIds.Count <= 0)
            {
                logger.LogInformation("CACHING: Cached stories are all up to date.");
                return currentCachedStories;
            }
            else
            {
                logger.LogInformation("CACHING: Getting information for {count} new stories that are not in cache.", newUncachedStoryIds.Count);
                return await AppendNewStoriesToCacheAsync(currentCachedStories, newUncachedStoryIds);
            }
        }

        private async Task<List<Story>> AppendNewStoriesToCacheAsync(List<Story> cachedStories, List<int> newStoryIds)
        {
            var newStories = await GetStoryDetailsByIdsAsync(newStoryIds);

            var updatedStories = newStories
                .Concat(cachedStories)
                .Take(MaxCachedStoriesCount)
                .ToList();

            cache.Set(CachedStories, updatedStories, BaseCacheDuration);

            logger.LogInformation("CACHING: Cached stories have been updated.");

            return updatedStories;
        }

        private async Task<List<Story>> GetNewStoriesAsync()
        {
            var storyIds = await thirdPartyService.GetNewestStoryIdsAsync();

            var stories = await GetStoryDetailsByIdsAsync(storyIds);

            cache.Set(CachedStories, stories, BaseCacheDuration);

            logger.LogInformation("CACHING: New cached stories have been set.");

            return stories;
        }

        private async Task<List<Story>> GetStoryDetailsByIdsAsync(List<int> storyIds)
        {
            var storyTasks = storyIds
                .Take(MaxCachedStoriesCount)
                .Select(id => thirdPartyService.GetStoryDetailsAsync(id));

            var storyResults = await Task.WhenAll(storyTasks);

            var stories = storyResults
                .OfType<Story>()
                .Where(x => x.HasVisibleData())
                .ToList() ?? [];

            return stories;
        }
    }
}
