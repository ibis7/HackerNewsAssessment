using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAPI.Services
{
    public class SearchService(IMemoryCache cache, IStoriesService storiesService, ILogger<SearchService> logger) : ISearchService
    {
        private const string CachedStories = "NewestStories";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

        public async Task<SearchResponse> GetFilteredStoriesAsync(SearchRequest searchRequest)
        {
            var baseStories = await GetNewestStoriesAsync();

            if (searchRequest.IsSearching())
            {
                baseStories = FilterStories(searchRequest, baseStories);
            }

            var orderedStories = OrderStories(searchRequest, baseStories);

            return new SearchResponse
            {
                Stories = orderedStories,
                TotalLength = baseStories.Count
            };
        }

        private async Task<List<Story>> GetNewestStoriesAsync()
        {
            //TODO: Append new ones to cache, always go to API?

            if (cache.TryGetValue(CachedStories, out List<Story>? cachedStories) && cachedStories != null)
            {
                logger.LogInformation("Requested from cached stories.");
                return cachedStories;
            }
            else
            {
                logger.LogInformation("Cached stories are unavailable/expired.");
                return await GetStoriesWithDetailsFromApiAsync();
            }
        }

        private async Task<List<Story>> GetStoriesWithDetailsFromApiAsync()
        {
            var storyIds = await storiesService.GetNewestStoryIdsAsync();

            //TODO: Only get the ones we are going to show??

            var storyTasks = storyIds.Select(id => storiesService.GetStoryDetailsAsync(id));
            var storyResults = await Task.WhenAll(storyTasks);

            var stories = storyResults
                .OfType<Story>()
                .Where(x => x.HasVisibleData())
                .ToList() ?? [];

            cache.Set(CachedStories, stories, CacheDuration);
            logger.LogInformation("New cached stories have been set.");

            return stories;
        }

        private static List<Story> FilterStories(SearchRequest searchRequest, List<Story> storiesUnfiltered)
        {
            var searchTerm = searchRequest.SearchTerm!.Trim();

            return storiesUnfiltered.Where(x =>
                (!string.IsNullOrEmpty(x.Url) && x.Url.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
        }

        private static List<Story> OrderStories(SearchRequest searchRequest, List<Story> stories)
        {
            var query = stories.AsQueryable();

            if (searchRequest.IsSorting())
            {
                var isAsc = searchRequest.IsSortingAscending!.Value;

                query = searchRequest.SortedBy! switch
                {
                    SortableColumnsEnum.Url => isAsc ? query.OrderBy(x => x.Url) : query.OrderByDescending(x => x.Url),
                    SortableColumnsEnum.Title => isAsc ? query.OrderBy(x => x.Title) : query.OrderByDescending(x => x.Title),
                    _ => query
                };
            }

            query = query
                .Skip(searchRequest.PageSize * searchRequest.PageNumber)
                .Take(searchRequest.PageSize);

            return query.ToList();
        }
    }
}
