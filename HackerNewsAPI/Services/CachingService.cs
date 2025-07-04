using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAPI.Services
{
    public class CachingService(IMemoryCache cache, IThirdPartyService thirdPartyService, ILogger<CachingService> logger) : ICachingService
    {
        private const string CachedStories = "NewestStories";
        private static readonly TimeSpan BaseCacheDuration = TimeSpan.FromSeconds(45);
        private static readonly TimeSpan UpdatedCacheDuration = TimeSpan.FromSeconds(10);

        public async Task<List<Story>> GetNewestStoriesAsync()
        {
            //TODO: Only get the ones we are going to show??

            if (cache.TryGetValue(CachedStories, out List<Story>? cachedStories) && cachedStories != null)
            {
                logger.LogInformation("Requested from cached stories.");
                return cachedStories;
                //return await GetCachedStoriesAsync(cachedStories);
            }
            else
            {
                logger.LogInformation("Cached stories are unavailable/expired.");
                return await GetNewStoriesAsync();
            }
        }

        private async Task<List<Story>> GetCachedStoriesAsync(List<Story> cachedStories)
        {
            var allStoryIds = await thirdPartyService.GetNewestStoryIdsAsync();

            var newStoryIds = allStoryIds.Except(cachedStories.Select(x => x.Id)).ToList();

            if (newStoryIds.Count <= 0)
            {
                return cachedStories;
            }
            else
            {
                logger.LogInformation("Getting information for {count} new stories that are not in cache.", newStoryIds.Count);

                return await AppendNewStoriesToCacheAsync(cachedStories, newStoryIds);
            }
        }

        private async Task<List<Story>> AppendNewStoriesToCacheAsync(List<Story> cachedStories, List<int> newStoryIds)
        {
            var newStoryTasks = newStoryIds.Select(id => thirdPartyService.GetStoryDetailsAsync(id));
            var newStoryResults = await Task.WhenAll(newStoryTasks);

            var newStories = newStoryResults
                .OfType<Story>()
                .Where(x => x.HasVisibleData())
                .ToList() ?? [];

            var updatedStories = cachedStories.Concat(newStories).ToList();

            cache.Set(CachedStories, updatedStories, UpdatedCacheDuration);
            logger.LogInformation("Cached stories have been updated.");
            return updatedStories;
        }

        private async Task<List<Story>> GetNewStoriesAsync()
        {
            var storyIds = await thirdPartyService.GetNewestStoryIdsAsync();

            var storyTasks = storyIds.Select(id => thirdPartyService.GetStoryDetailsAsync(id));
            var storyResults = await Task.WhenAll(storyTasks);

            var stories = storyResults
                .OfType<Story>()
                .Where(x => x.HasVisibleData())
                .ToList() ?? [];

            cache.Set(CachedStories, stories, BaseCacheDuration);
            logger.LogInformation("New cached stories have been set.");

            return stories;
        }
    }
}
